using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.iOS.Central
{
    public class CentralManager : ICentralManager
    {
        private DispatchQueue _dispatchQueue;
        private CBCentralManager _centralManager;
        private CentralManagerDelegate.CentralManagerDelegate _centralManagerDelegate;
        private bool _initialized;

        public ManagerState State => _initialized ? ManagerState.PoweredOn : ManagerState.PoweredOff;

        public CentralManager()
        {
            _centralManagerDelegate = new CentralManagerDelegate.CentralManagerDelegate();
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
            return _centralManagerDelegate.StateUpdatedSubject.Select(x => (ManagerState) x);
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
                        Device device = new Device(x.Peripheral);
                        return device;
                    }).Subscribe(observer);

                if (!_centralManager.IsScanning)
                    _centralManager.ScanForPeripherals((CBUUID[]) null);
                return Disposable.Create(() => { _centralManager.StopScan(); });
            });
        }

        public Task<bool> ConnectToDevice(IDevice device)
        {
            CheckInitialized();
            var nativeDevice = ((Device) device).NativeDevice;

            var connectedObservable = _centralManagerDelegate.ConnectedPeripheralSubject
                .Select(x => true);
            var connectionErrorObservable = _centralManagerDelegate.FailedToConnectPeripheralSubject
                .Select(x => false);

            var merged = connectionErrorObservable.Merge(connectedObservable);
            _centralManager.ConnectPeripheral(nativeDevice);

            return merged.FirstAsync().ToTask();
        }

        public Task DisconnectDevice(IDevice device)
        {
            var nativeDevice = ((Device) device).NativeDevice;
            _centralManager.CancelPeripheralConnection(nativeDevice);
            return Task.FromResult(true);
        }

        public IObservable<IService> DiscoverServices(IDevice device)
        {
            var nativeDevice = ((Device) device).NativeDevice;
            nativeDevice.DiscoverServices();
            throw new NotImplementedException();
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