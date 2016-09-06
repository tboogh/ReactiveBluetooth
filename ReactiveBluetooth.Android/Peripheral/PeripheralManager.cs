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
using Plugin.CurrentActivity;
using ReactiveBluetooth.Android.Common;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Android.Peripheral.GattServer;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;
using ReactiveBluetooth.Core.Peripheral;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class PeripheralManager : IPeripheralManager
    {
        private BroadcastListener _broadcastListener;
        private readonly BluetoothAdapter _bluetoothAdapter;
        private BluetoothLeAdvertiser _bluetoothLeAdvertiser;
        private readonly IServerCallback _serverCallback;
        private BluetoothGattServer _gattServer;
        private IObservable<bool> _startAdvertisingObservable;

        public PeripheralManager() :this(null, null)
        {
            
        }

        public PeripheralManager(IServerCallback serverCallback = null, IBluetoothAbstractFactory bluetoothAbstractFactory = null)
        {
            _serverCallback = serverCallback ?? new ServerCallback();
            Factory = bluetoothAbstractFactory ?? new AbstractFactory(_serverCallback);

            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _broadcastListener = new BroadcastListener();
        }

        public IBluetoothAbstractFactory Factory { get; }

        public IObservable<ManagerState> State()
        {
            return _broadcastListener.StateUpdatedSubject.Select(x => x.ToManagerState());
        } 

        public void Shutdown()
        {
            _gattServer.Close();
            _broadcastListener.Dispose();
            _broadcastListener = null;
        }

        public IObservable<bool> Advertise(AdvertisingOptions advertisingOptions, IList<IService> services)
        {
            if (_startAdvertisingObservable != null)
            {
                return _startAdvertisingObservable;
            }

            if (advertisingOptions.LocalName != null)
            {
                _bluetoothAdapter.SetName(advertisingOptions.LocalName);
            }

            _bluetoothLeAdvertiser = _bluetoothAdapter.BluetoothLeAdvertiser;

            if (_bluetoothLeAdvertiser == null)
            {
                throw new AdvertisingNotSupportedException();
            }

            var settings = CreateAdvertiseSettings();
            var advertiseData = CreateAdvertiseData(advertisingOptions);

            var startObservable = Observable.Create<bool>(observer =>
            {

                var callback = new StartAdvertiseCallback();
                var errorDisposable = callback.StartFailureSubject.Subscribe(failure =>
                {
                    if (failure != 0)
                    {
                        observer.OnError(new AdvertiseException(failure.ToString()));
                    }
                });

                var successDisposable = callback.StartSuccessSubject.Subscribe(advertiseSettings =>
                {
                    observer.OnNext(true);
                });

                if (_gattServer == null)
                {
                    var bluetoothManager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
                    _gattServer = bluetoothManager.OpenGattServer(Application.Context, (BluetoothGattServerCallback) _serverCallback);
                }

                foreach (var service in services)
                {
                    AddService(service);
                }

                _bluetoothLeAdvertiser.StartAdvertising(settings, advertiseData, callback);
                return Disposable.Create(() =>
                {
                    errorDisposable.Dispose();
                    successDisposable.Dispose();
                    _gattServer.Close();
                    _bluetoothLeAdvertiser.StopAdvertising(callback);
                    _startAdvertisingObservable = null;
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