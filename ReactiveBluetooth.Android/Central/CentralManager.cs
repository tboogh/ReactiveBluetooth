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
        private readonly BluetoothAdapter _bluetoothAdapter;
        private IObservable<IDevice> _discoverObservable;
        private BroadcastListener _broadcastListener;

        public CentralManager()
        {
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _broadcastListener = new BroadcastListener();
        }

        public void Dispose()
        {
            _broadcastListener.Dispose();
            _broadcastListener = null;
        }

        public IObservable<ManagerState> State()
        {
            return _broadcastListener.StateUpdatedSubject.Select(state =>
            {
                var s = state.ToManagerState();
                return s;
            }).StartWith(_bluetoothAdapter.State.ToManagerState());
        }

        public IObservable<IDevice> ScanForDevices()
        {
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
            var androidDevice = (Device) device;
            var nativeDevice = androidDevice.NativeDevice;
            var context = Application.Context;

            var gatt = nativeDevice.ConnectGatt(context, false, androidDevice.GattCallback);
            androidDevice.Gatt = gatt;
            return androidDevice.GattCallback.ConnectionStateChange.Select(x => (ConnectionState) x);
        }

        public Task DisconnectDevice(IDevice device)
        {
            var androidDevice = (Device) device;
            androidDevice.Gatt?.Close();
            return Task.FromResult(true);
        }

        public void DiscoverServices(IDevice device)
        {
            var androidDevice = (Device) device;
            androidDevice.Gatt.DiscoverServices();
        }
    }
}