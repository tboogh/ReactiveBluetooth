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
using ReactiveBluetooth.Android.Peripheral.GattServer;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Peripheral;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class PeripheralManager : IPeripheralManager
    {
        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothLeAdvertiser _bluetoothLeAdvertiser;
        private readonly ServerCallback _serverCallback;
        private BluetoothGattServer _gattServer;

        public PeripheralManager()
        {
            if (_serverCallback == null)
            {
                _serverCallback = new ServerCallback();
            }
            Factory = new AbstractFactory(_serverCallback);
        }

        public IBluetoothAbstractFactory Factory { get; }

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
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _bluetoothLeAdvertiser = _bluetoothAdapter.BluetoothLeAdvertiser;

            return Observable
                .Timer(TimeSpan.FromSeconds(0.5))
                .Select(x => State).StartWith(State);
        }

        public void Shutdown()
        {
            _gattServer.Close();
        }

        public IObservable<bool> StartAdvertising(AdvertisingOptions advertisingOptions, IList<IService> services)
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
                }};
                if (_gattServer == null)
                {
                    var bluetoothManager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
                    _gattServer = bluetoothManager.OpenGattServer(Application.Context, _serverCallback);
                }

                foreach (var service in services)
                {
                    AddService(service);
                }

                _bluetoothLeAdvertiser.StartAdvertising(settings, advertiseData, callback);
                return Disposable.Create(() =>
                {
                    _gattServer.Close();
                    _gattServer = null;
                    _bluetoothLeAdvertiser.StopAdvertising(callback);
                });
            });
            return startObservable;
        }

        public AdvertiseData CreateAdvertiseData(AdvertisingOptions advertisingOptions)
        {
            
            var dataBuilder = new AdvertiseData.Builder();
            if (advertisingOptions.ServiceUuids != null)
            {
                var parcelUuids = advertisingOptions.ServiceUuids.Select(x => ParcelUuid.FromString(x.ToString()));
                foreach (var parcelUuid in parcelUuids)
                {
                    dataBuilder.AddServiceUuid(parcelUuid);
                }
            }
            dataBuilder.SetIncludeDeviceName(advertisingOptions.LocalName != null);

         
            return dataBuilder.Build();
        }

        public AdvertiseSettings CreateAdvertiseSettings()
        {
            AdvertiseSettings.Builder settingsBuilder = new AdvertiseSettings.Builder();
            settingsBuilder.SetAdvertiseMode(AdvertiseMode.Balanced);
            settingsBuilder.SetConnectable(true);
            settingsBuilder.SetTxPowerLevel(AdvertiseTx.PowerMedium);
            settingsBuilder.SetTimeout(5000);
            var settings = settingsBuilder.Build();
            return settings;
        }

        public IObservable<bool> AddService(IService service)
        {
            var nativeService = ((Service) service).GattService;
            var result = _gattServer?.AddService(nativeService);
            return Observable.Return(result ?? false);
        }

        public void RemoveService(IService service)
        {
            var nativeService = ((Service)service).GattService;
            _gattServer?.RemoveService(nativeService);
        }

        public void RemoveAllServices()
        {
            _gattServer?.ClearServices();
        }

        public bool SendResponse(IAttRequest request, int offset, byte[] value)
        {
            AttRequest attRequest = (AttRequest) request;
            return _gattServer.SendResponse(attRequest.BluetoothDevice, attRequest.RequestId, GattStatus.Success, offset, value);
        }
    }
}