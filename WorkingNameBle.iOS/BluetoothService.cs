using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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
        private CBCentralManager _peripheralManager;
        private IObservable<EventPattern<object>> _updateStateObservable;
        private IObservable<Device> _disoverDeviceObservable;
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
            _peripheralManager = new CBCentralManager(_dispatchQueue);

            if (_updateStateObservable != null)
            {
                _updateStateObservable = Observable.FromEventPattern(eh => _peripheralManager.UpdatedState += eh, eh => _peripheralManager.UpdatedState -= eh);
                _readyTaskCompletionSource = new TaskCompletionSource<bool>();
                _updateStateDisposable = _updateStateObservable.Subscribe(pattern =>
                {
                    switch (_peripheralManager.State)
                    {
                        case CBCentralManagerState.Unknown:
                        case CBCentralManagerState.PoweredOff:
                        case CBCentralManagerState.Resetting:
                            // Ignore therse
                            break;
                        case CBCentralManagerState.Unsupported:
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
            return _readyTaskCompletionSource.Task;
        }

        public IObservable<IDevice> ScanForDevices()
        {
            if (_peripheralManager.State != CBCentralManagerState.PoweredOn)
            {
                // Throw? return null? handle...
            }
            if (!_peripheralManager.IsScanning)
                _peripheralManager.ScanForPeripherals((CBUUID[])null);

            return Observable.FromEventPattern<EventHandler<CBDiscoveredPeripheralEventArgs>, CBDiscoveredPeripheralEventArgs>(eh => _peripheralManager.DiscoveredPeripheral += eh, eh => _peripheralManager.DiscoveredPeripheral -= eh)
                .Select(x =>
                {
                    Device device = new Device(x.EventArgs.Peripheral);
                    return device;
                });
        }
    }
}