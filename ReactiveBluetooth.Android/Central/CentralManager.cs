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
using Object = Java.Lang.Object;

namespace ReactiveBluetooth.Android.Central
{
    public class CentralManager : ICentralManager
    {
        private readonly BluetoothAdapter _bluetoothAdapter;
        private IObservable<IDevice> _discoverObservable;
        private BroadcastListener _broadcastListener;
        private BluetoothManager _bluetoothManager;

        public CentralManager()
        {
            _bluetoothManager = (BluetoothManager) Application.Context.GetSystemService(Context.BluetoothService);
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
            })
                .StartWith(_bluetoothAdapter.State.ToManagerState());
        }

        public IObservable<IDevice> ScanForDevices()
        {
            if (_discoverObservable == null)
            {
                // Store this and return the same someone else subscribes
                BleScanCallback bleScanCallback = new BleScanCallback();

                _discoverObservable = Observable.FromEvent<IDevice>(action =>
                {
                    _bluetoothAdapter.BluetoothLeScanner.StartScan(bleScanCallback);
                }, action => { _bluetoothAdapter.BluetoothLeScanner.StopScan(bleScanCallback); })
                    .Merge(bleScanCallback.ScanResultSubject.Select(x => new Device(x.Item2.Device, x.Item2.Rssi)))
                    .Merge<IDevice>(bleScanCallback.FailureSubject.Select(failure =>
                    {
                        throw new Exception(failure.ToString());
                        return default(Device);
                    }));
            }

            return _discoverObservable;
        }

        public IObservable<ConnectionState> ConnectToDevice(IDevice device)
        {
            var androidDevice = (Device) device;
            var nativeDevice = androidDevice.NativeDevice;
            var context = Application.Context;

            return Observable.FromEvent<ConnectionState>(action =>
            {
                var gatt = nativeDevice.ConnectGatt(context, false, androidDevice.GattCallback);
                androidDevice.Gatt = gatt;
            }, action =>
            {
                androidDevice.Gatt?.Close(); 
            })
                .Merge(androidDevice.GattCallback.ConnectionStateChange.Select(x => (ConnectionState) x));
        }
    }
}