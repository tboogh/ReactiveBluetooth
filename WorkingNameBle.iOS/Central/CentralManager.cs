using System;
using System.Reactive.Concurrency;
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
        private bool _initialized;

        public ManagerState State => _initialized ?  ManagerState.PoweredOn : ManagerState.PoweredOff;

        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            if (_initialized)
            {
                throw new NotSupportedException("Manager already initialized");
            }

            _dispatchQueue = new DispatchQueue("com.workingble.periperhalmanager.queue");
            _centralManager = new CBCentralManager(_dispatchQueue);

            _initialized = true;
            return Observable.FromEventPattern(eh => _centralManager.UpdatedState += eh, eh => _centralManager.UpdatedState -= eh)
                .Select(x => (ManagerState)_centralManager.State)
                .StartWith((ManagerState)_centralManager.State);
            ;
        }

        public void Shutdown()
        {
            _initialized = false;
        }
        
        public IObservable<IDevice> ScanForDevices()
        {
            CheckInitialized();
            if (_centralManager.State != CBCentralManagerState.PoweredOn)
            {
                // Throw? return null? handle...
            }
            if (!_centralManager.IsScanning)
                _centralManager.ScanForPeripherals((CBUUID[])null);

            return Observable.FromEventPattern<EventHandler<CBDiscoveredPeripheralEventArgs>, CBDiscoveredPeripheralEventArgs>(eh => _centralManager.DiscoveredPeripheral += eh, eh => _centralManager.DiscoveredPeripheral -= eh)
                .Select(x =>
                {
                    Device device = new Device(x.EventArgs.Peripheral);
                    return device;
                });
        }

        public Task<bool> ConnectToDevice(IDevice device)
        {
            CheckInitialized();
            var nativeDevice = ((Device) device).NativeDevice;
            
            var connectedObservable = Observable.FromEventPattern<EventHandler<CBPeripheralEventArgs>, CBPeripheralEventArgs>(eh => _centralManager.ConnectedPeripheral += eh, eh => _centralManager.ConnectedPeripheral -= eh)
                .Select(x => true);
            var connectionErrorObservable = Observable.FromEventPattern<EventHandler<CBPeripheralErrorEventArgs>, CBPeripheralErrorEventArgs>(eh => _centralManager.FailedToConnectPeripheral += eh, eh => _centralManager.FailedToConnectPeripheral -= eh)
                .Select(x => false);

            var merged = connectionErrorObservable.Merge(connectedObservable);
            _centralManager.ConnectPeripheral(nativeDevice);

            return merged.FirstAsync().ToTask();
        }

        public Task DisconnectDevice(IDevice device)
        {
            var nativeDevice = ((Device)device).NativeDevice;
            _centralManager.CancelPeripheralConnection(nativeDevice);
            return Task.FromResult(true);
        }

        public IObservable<IService> DiscoverServices(IDevice device)
        {
            var nativeDevice = ((Device)device).NativeDevice;
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