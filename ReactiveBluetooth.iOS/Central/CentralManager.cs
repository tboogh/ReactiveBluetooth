using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;

namespace ReactiveBluetooth.iOS.Central
{
    public class CentralManager : ICentralManager
    {
        private DispatchQueue _dispatchQueue;
        private CBCentralManager _centralManager;
        private CentralManagerDelegate.CentralManagerDelegate _centralManagerDelegate;
        private bool _initialized;

        public CentralManager()
        {
            _centralManagerDelegate = new CentralManagerDelegate.CentralManagerDelegate();
        }

        public ManagerState State => _initialized ? ManagerState.PoweredOn : ManagerState.PoweredOff;

        public IObservable<ManagerState> StateUpdates()
        {
            return _centralManagerDelegate.StateUpdatedSubject.Select(x => (ManagerState)x);
        }

        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            if (_initialized)
            {
                throw new NotSupportedException("Manager already initialized");
            }

            _dispatchQueue = new DispatchQueue("com.workingble.periperhalmanager.queue");
            _centralManager = new CBCentralManager(_centralManagerDelegate, _dispatchQueue);

            _initialized = true;
            return StateUpdates();
        }

        public void Shutdown()
        {
            _initialized = false;
        }

        public IObservable<IDevice> ScanForDevices()
        {
            CheckInitialized();

            return Observable.Create<IDevice>(observer =>
            {
                _centralManagerDelegate.DiscoveredPeriperhalSubject
                    .Select(x =>
                    {
                        Device device = new Device(x.Item2, x.Item4.Int32Value);
                        return device;
                    }).Subscribe(observer);

                if (!_centralManager.IsScanning)
                    _centralManager.ScanForPeripherals((CBUUID[]) null);
                return Disposable.Create(() => { _centralManager.StopScan(); });
            });
        }

        public IObservable<ConnectionState> ConnectToDevice(IDevice device)
        {
            CheckInitialized();
            var nativeDevice = ((Device) device).Peripheral;

            return Observable.Create<ConnectionState>(observer =>
            {
                var connectedObservable = _centralManagerDelegate.ConnectedPeripheralSubject
                    .Where(x => Equals(x.Item2.Identifier, nativeDevice.Identifier))
                    .Select(x => ConnectionState.Connected);
                var disconnectObservable = _centralManagerDelegate.DisconnectedPeripheralSubject.Where(x => Equals(x.Item2.Identifier, nativeDevice.Identifier))
                    .Select(x => ConnectionState.Disconnected);
                var stateDisposable = Observable.Merge(connectedObservable, disconnectObservable).StartWith(ConnectionState.Connecting).Subscribe(observer);

                var connectionErrorDisp = _centralManagerDelegate.FailedToConnectPeripheralSubject
                .Select(x => x.Item2.Identifier == nativeDevice.Identifier)
                    .Subscribe(tuple =>
                    {
                        observer.OnError(new FailedToConnectException());
                    });

                _centralManager.ConnectPeripheral(nativeDevice);
                return Disposable.Create(() =>
                {
                    connectionErrorDisp.Dispose();
                    stateDisposable.Dispose();
                });
            });
        }

        public Task DisconnectDevice(IDevice device)
        {
            var peripheral = ((Device) device).Peripheral;
            _centralManager.CancelPeripheralConnection(peripheral);
            return _centralManagerDelegate.DisconnectedPeripheralSubject.FirstAsync(x => Equals(x.Item2.Identifier, peripheral.Identifier)).ToTask();
        }

        public void DiscoverServices(IDevice device)
        {
            var nativeDevice = ((Device) device).Peripheral;
            nativeDevice.DiscoverServices();
        }

        private void CheckInitialized()
        {
            if (!_initialized)
            {
                throw new Exception("Service not initialized");
            }
        }
    }
}