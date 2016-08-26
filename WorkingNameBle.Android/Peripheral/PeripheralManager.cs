using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Javax.Security.Auth;
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Peripheral;
using IService = WorkingNameBle.Core.Peripheral.IService;

namespace WorkingNameBle.Android.Peripheral
{
    public class ServerCallback : BluetoothGattServerCallback
    {
        
    }

    public class PeripheralManager : IPeripheralManager
    {
        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothLeAdvertiser _bluetoothLeAdvertiser;
        private ServerCallback _serverCallback;
        private BluetoothGattServer _gattServer;

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
            _serverCallback = new ServerCallback();
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _bluetoothLeAdvertiser = _bluetoothAdapter.BluetoothLeAdvertiser;
            var bluetoothManager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
            _gattServer = bluetoothManager.OpenGattServer(Application.Context, _serverCallback);

            return Observable
                .Timer(TimeSpan.FromSeconds(0.5))
                .Select(x => State).StartWith(State);
        }

        public void Shutdown()
        {
            
        }

        public IObservable<bool> StartAdvertising(AdvertisingOptions advertisingOptions)
        {
            if (advertisingOptions.LocalName != null)
            {
                _bluetoothAdapter.SetName(advertisingOptions.LocalName);
            }
            
            var settings = CreateAdvertiseSettings();
            var advertiseData = CreateAdvertiseData(advertisingOptions);

            var startObservable = Observable.Create<bool>(observer =>
            {
                var callback = new StartAdvertiseCallback {StartFailure = failure => observer.OnError(new Exception($"Advertise start failed: {Enum.GetName(typeof(AdvertiseFailure), failure)}")), StartSuccess = b =>
                {
                    observer.OnNext(b);
                    observer.OnCompleted();
                }};
                _bluetoothLeAdvertiser.StartAdvertising(settings, advertiseData, callback);
                return Disposable.Create(() =>
                {
                    _bluetoothLeAdvertiser.StopAdvertising(callback);
                });
            });
            return startObservable;
        }

        public AdvertiseData CreateAdvertiseData(AdvertisingOptions advertisingOptions)
        {
            var parcelUuids = advertisingOptions.ServiceUuids.Select(x => ParcelUuid.FromString(x.ToString()));
            var dataBuilder = new AdvertiseData.Builder();

            foreach (var parcelUuid in parcelUuids)
            {
                dataBuilder.AddServiceUuid(parcelUuid);
            }
            dataBuilder.SetIncludeDeviceName(advertisingOptions.LocalName != null);

         
            return dataBuilder.Build();
        }

        public AdvertiseSettings CreateAdvertiseSettings()
        {
            AdvertiseSettings settings;
            AdvertiseSettings.Builder settingsBuilder = new AdvertiseSettings.Builder();
            settingsBuilder.SetAdvertiseMode(AdvertiseMode.Balanced);
            settingsBuilder.SetConnectable(true);
            settingsBuilder.SetTxPowerLevel(AdvertiseTx.PowerMedium);
            settingsBuilder.SetTimeout(5000);
            settings = settingsBuilder.Build();
            return settings;
        }

        public void AddService(IService service)
        {
            var nativeService = ((Service) service).GattService;
            _gattServer.AddService(nativeService);
        }

        public void RemoveSerivce(IService service)
        {
            var nativeService = ((Service)service).GattService;
            _gattServer.RemoveService(nativeService);
        }

        public void RemoveAllServices()
        {
            _gattServer.ClearServices();
        }
    }
}