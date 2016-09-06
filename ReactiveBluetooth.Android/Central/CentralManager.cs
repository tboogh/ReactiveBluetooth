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
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;

namespace ReactiveBluetooth.Android.Central
{
    public class CentralManager : ICentralManager
    {
        private BluetoothAdapter _bluetoothAdapter;
        private bool _initialized;
        private IObservable<IDevice> _discoverObservable;
        private readonly BleBroadcastReciever _broadcastReciever;
        public ManagerState State => StateToManagerState(_bluetoothAdapter.State);

        public CentralManager()
        {
            _broadcastReciever = new BleBroadcastReciever();
        }

        private ManagerState StateToManagerState(State state)
        {
            switch (state)
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

        public IObservable<ManagerState> StateUpdates()
        {
            return _broadcastReciever.StateUpdatedSubject.Select(StateToManagerState).StartWith(State);
        }

        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            if (!_initialized)
            {
                CrossCurrentActivity.Current.Activity.RegisterReceiver(_broadcastReciever, new IntentFilter(BluetoothAdapter.ActionStateChanged));
                _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            }
            _initialized = true;

            return StateUpdates();
        }

        public void Shutdown()
        {
            CrossCurrentActivity.Current.Activity.UnregisterReceiver(_broadcastReciever);
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