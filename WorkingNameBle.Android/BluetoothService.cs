using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Exceptions;

namespace WorkingNameBle.Android
{
    public class BluetoothService : IBluetoothService
    {
        private BluetoothAdapter _bluetoothAdapter;
        private bool _initialized;
        private IObservable<IDevice> _discoverObservable;

        public void Init(IScheduler scheduler = null)
        {
            if (!_initialized)
            {
                _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            }
            _initialized = true;
        }

        public void Shutdown()
        {
            _initialized = false;
        }

        public Task<bool> ReadyToDiscover()
        {
            return Task.FromResult(_initialized);
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