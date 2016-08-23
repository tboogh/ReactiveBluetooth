using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Exceptions;

namespace WorkingNameBle.iOS
{
    public class BluetoothService : IBluetoothService
    {
        private DispatchQueue _dispatchQueue;
        private CBCentralManager _centralManager;
        private IObservable<EventPattern<object>> _updateStateObservable;
        private IDisposable _updateStateDisposable;
        private TaskCompletionSource<bool> _readyTaskCompletionSource;
        private bool _initialized;

        public void Init(IScheduler scheduler = null)
        {
            if (_initialized)
            {
                return;
            }

            _dispatchQueue = new DispatchQueue("com.workingble.periperhalmanager.queue");
            _centralManager = new CBCentralManager(_dispatchQueue);

            if (_updateStateObservable != null)
            {
                _updateStateObservable = Observable.FromEventPattern(eh => _centralManager.UpdatedState += eh, eh => _centralManager.UpdatedState -= eh);
                _readyTaskCompletionSource = new TaskCompletionSource<bool>();
                _updateStateDisposable = _updateStateObservable.Subscribe(pattern =>
                {
                    switch (_centralManager.State)
                    {
                        case CBCentralManagerState.Unknown:
                        case CBCentralManagerState.PoweredOff:
                        case CBCentralManagerState.Resetting:
                            // Ignore therse
                            break;
                        case CBCentralManagerState.Unsupported:
                            _readyTaskCompletionSource.SetResult(false);
                            throw new DiscoverDeviceException("BLE mode not authorized");
                        case CBCentralManagerState.Unauthorized:
                            _readyTaskCompletionSource.SetResult(false);
                            throw new DiscoverDeviceException("BLE mode not supported");
                        case CBCentralManagerState.PoweredOn:
                            _readyTaskCompletionSource?.SetResult(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }

            _initialized = true;
        }

        public void Shutdown()
        {
            _updateStateDisposable?.Dispose();
            _updateStateObservable = null;
            _readyTaskCompletionSource?.SetResult(false);
            _readyTaskCompletionSource = null;
            _initialized = false;
        }

        public Task<bool> ReadyToDiscover()
        {
            CheckInitialized();
            // Add timeout
            return _readyTaskCompletionSource.Task;
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