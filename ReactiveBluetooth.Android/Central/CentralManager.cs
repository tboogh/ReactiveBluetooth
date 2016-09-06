using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Plugin.CurrentActivity;
using ReactiveBluetooth.Android.Common;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;

namespace ReactiveBluetooth.Android.Central
{
    public class CentralManager : ICentralManager
    {
        private BluetoothAdapter _bluetoothAdapter;
        private bool _initialized;
        private IObservable<IDevice> _discoverObservable;
        private BroadcastListener _broadcastListener;
        public ManagerState State => _bluetoothAdapter.State.ToManagerState();

        public CentralManager()
        {
            
        }

        public IObservable<ManagerState> StateUpdates()
        {
            return _broadcastListener.StateUpdatedSubject.Select(state => state.ToManagerState()).StartWith(State);
        }

        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            if (!_initialized)
            {
                _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                _broadcastListener = new BroadcastListener();
            }
            _initialized = true;

            return StateUpdates();
        }

        public void Shutdown()
        {
            _initialized = false;
            _broadcastListener.Dispose();
            _broadcastListener = null;
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
                        var device = new Device(result.Device, result.Rssi);
                        observer.OnNext(device);
                    }, failure => { observer.OnError(new DiscoverDeviceException(failure.ToString())); });

                    _bluetoothAdapter.BluetoothLeScanner.StartScan(bleScanCallback);
                    return Disposable.Create(() => { _bluetoothAdapter.BluetoothLeScanner.StopScan(bleScanCallback); });
                });
            }
            return _discoverObservable;
        }

        public IObservable<ConnectionState> ConnectToDevice(IDevice device)
        {
            CheckInitialized();
            var androidDevice = (Device) device;
            var nativeDevice = androidDevice.NativeDevice;
            var context = Application.Context;

            var gatt = nativeDevice.ConnectGatt(context, false, androidDevice.GattCallback);
            androidDevice.Gatt = gatt;
            return androidDevice.GattCallback.ConnectionStateChange.Select(x => (ConnectionState) x);
        }

        public Task DisconnectDevice(IDevice device)
        {
            CheckInitialized();
            var androidDevice = (Device) device;

            androidDevice.Gatt?.Close();
            return Task.FromResult(true);
        }

        public void DiscoverServices(IDevice device)
        {
            CheckInitialized();
            var androidDevice = (Device) device;
            androidDevice.Gatt.DiscoverServices();
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