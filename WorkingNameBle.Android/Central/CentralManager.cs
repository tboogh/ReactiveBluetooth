using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Exceptions;

namespace WorkingNameBle.Android.Central
{
    public class CentralManager : ICentralManager
    {
        private BluetoothAdapter _bluetoothAdapter;
        private bool _initialized;
        private IObservable<IDevice> _discoverObservable;

        public ManagerState State
        {
            get
            {
                switch (_bluetoothAdapter.State)
                {
                    case global::Android.Bluetooth.State.Connected:
                    case global::Android.Bluetooth.State.Connecting:
                    case global::Android.Bluetooth.State.Disconnected:
                    case global::Android.Bluetooth.State.Disconnecting:
                    case global::Android.Bluetooth.State.On:
                        return ManagerState.PoweredOn;
                    case global::Android.Bluetooth.State.Off:
                        return ManagerState.PoweredOff;
                    case global::Android.Bluetooth.State.TurningOff:
                    case global::Android.Bluetooth.State.TurningOn:
                        return ManagerState.Resetting;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            if (!_initialized)
            {
                _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            }
            _initialized = true;
            
            return Observable
                .Timer(TimeSpan.FromSeconds(0.5))
                .Select(x => State);
        }

        public void Shutdown()
        {
            _initialized = false;
        }

        public IObservable<IDevice> ScanForDevices()
        {
            CheckInitialized();

            if (_discoverObservable == null)
            {
                // Store this and return the same someone else subscribes
                _discoverObservable = Observable.Create<IDevice>(observer =>
                {
                    BleScanCallback bleScanCallback = new BleScanCallback((type, result) =>
                    {
                        var device = new Device(result.Device);
                        observer.OnNext(device);
                    }, failure => { observer.OnError(new DiscoverDeviceException(failure.ToString())); });

                    _bluetoothAdapter.BluetoothLeScanner.StartScan(bleScanCallback);
                    return Disposable.Create(() => { _bluetoothAdapter.BluetoothLeScanner.StopScan(bleScanCallback); });
                });
            }
            return _discoverObservable;
        }

        public Task<bool> ConnectToDevice(IDevice device)
        {
            CheckInitialized();
            var androidDevice = (Device) device;
            var nativeDevice = androidDevice.NativeDevice;
            var context = Application.Context;

            var connectionObservable = Observable.Create<bool>(observer =>
            {
                var callback = new BleGattCallback {ConnectionStateChange = (gatt, status, newState) =>
                {
                    switch (status)
                    {
                        case GattStatus.ConnectionCongested:
                        case GattStatus.Failure:
                        case GattStatus.InsufficientAuthentication:
                        case GattStatus.InsufficientEncryption:
                        case GattStatus.InvalidAttributeLength:
                        case GattStatus.InvalidOffset:
                        case GattStatus.WriteNotPermitted:
                        case GattStatus.ReadNotPermitted:
                        case GattStatus.RequestNotSupported:
                            observer.OnError(new Exception($"Connection failed {status.ToString()}"));
                            break;
                        case GattStatus.Success:
                            androidDevice.Gatt = gatt;
                            observer.OnNext(true);
                            observer.OnCompleted();
                            break;

                        default:
                            observer.OnError(new Exception($"Unknown Status {status.ToString()}"));
                            break;
                    }
                }};
                androidDevice.Callback = callback;
                nativeDevice.ConnectGatt(context, false, callback);

                return Disposable.Create(() => { callback.ConnectionStateChange = null; });
            });

            return connectionObservable.ToTask();
        }

        public Task DisconnectDevice(IDevice device)
        {
            CheckInitialized();
            var androidDevice = (Device) device;

            androidDevice.Gatt?.Close();
            return Task.FromResult(true);
        }

        public IObservable<IService> DiscoverServices(IDevice device)
        {
            CheckInitialized();
            var androidDevice = (Device) device;
            androidDevice.Gatt.DiscoverServices();
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