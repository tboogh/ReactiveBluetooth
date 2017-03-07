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
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;
using IDevice = ReactiveBluetooth.Core.Peripheral.IDevice;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class PeripheralManager : IPeripheralManager, IDisposable
    {
        private BroadcastListener _broadcastListener;
        private readonly BluetoothAdapter _bluetoothAdapter;
        private BluetoothLeAdvertiser _bluetoothLeAdvertiser;
        private readonly IServerCallback _serverCallback;
        private BluetoothGattServer _gattServer;
        private IObservable<bool> _startAdvertisingObservable;
        private ManagerState _lastState;
        
        public PeripheralManager() : this(null, null)
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
        public IObservable<ManagerState> State() => _broadcastListener.StateUpdatedSubject.Select(x =>
        {
            if (x != ManagerState.PoweredOn)
                return x;

            return _bluetoothAdapter?.BluetoothLeAdvertiser == null ? ManagerState.Unsupported : x;
        }).AsObservable();

        public void Dispose()
        {
            _gattServer?.Close();
            _broadcastListener?.Dispose();
            _broadcastListener = null;
        }

        public IObservable<bool> Advertise(AdvertisingOptions advertisingOptions, IList<IService> services)
        {
            if (_startAdvertisingObservable != null)
            {
                return _startAdvertisingObservable;
            }

            var advertiseObservable = Observable.Create<bool>(observer =>
            {
                var bluetoothManager = (BluetoothManager)Application.Context.GetSystemService(Context.BluetoothService);
                _gattServer = bluetoothManager.OpenGattServer(CrossCurrentActivity.Current.Activity, (BluetoothGattServerCallback)_serverCallback);

                if (_bluetoothAdapter.State.ToManagerState() == ManagerState.PoweredOff)
                {
                    observer.OnError(new Exception("Device is off"));
                    return Disposable.Empty;
                }

                _bluetoothLeAdvertiser = _bluetoothAdapter.BluetoothLeAdvertiser;

                if (_bluetoothLeAdvertiser == null)
                {
                    observer.OnError(new AdvertisingNotSupportedException());
                    return Disposable.Empty;
                }

                if (advertisingOptions.LocalName != null)
                {
                    _bluetoothAdapter.SetName(advertisingOptions.LocalName);
                }

                var settings = CreateAdvertiseSettings(advertisingOptions);
                var advertiseData = CreateAdvertiseData(advertisingOptions);

                var callback = new StartAdvertiseCallback();

                var successDisposable = callback.AdvertiseSubject.Subscribe(advertiseSettings => { observer.OnNext(true); }, observer.OnError);

                if (services != null)
                {
                    foreach (var service in services)
                    {
                        AddService(service);
                    }
                }
                try
                {
                    _bluetoothLeAdvertiser.StartAdvertising(settings, advertiseData, callback);
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                    return Disposable.Empty;
                }
                

                return Disposable.Create(() =>
                {
                    successDisposable?.Dispose();
                    _gattServer?.Close();
                    _bluetoothLeAdvertiser?.StopAdvertising(callback);
                    _startAdvertisingObservable = null;
                });
            })
                .Publish()
                .RefCount();
            return advertiseObservable;
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

        public AdvertiseSettings CreateAdvertiseSettings(AdvertisingOptions advertisingOptions)
        {
            AdvertiseSettings.Builder settingsBuilder = new AdvertiseSettings.Builder();
            settingsBuilder.SetAdvertiseMode(advertisingOptions.AdvertiseMode.ToAdvertiseMode());
            settingsBuilder.SetConnectable(true);
            settingsBuilder.SetTxPowerLevel(advertisingOptions.AdvertiseTx.ToAdvertiseTx());
            var settings = settingsBuilder.Build();
            return settings;
        }

        public IObservable<bool> AddService(IService service)
        {
            var androidService = ((Service) service);
            if (androidService.Characteristics != null)
                foreach (var characteristic in androidService.Characteristics)
                {
                    var androidCharacteristic = (Characteristic) characteristic;
                    androidCharacteristic.GattServer = _gattServer;
                }
            var nativeService = androidService.GattService;
            var result = _gattServer?.AddService(nativeService);
            return Observable.Return(result ?? false);
        }

        public void RemoveService(IService service)
        {
            var nativeService = ((Service) service).GattService;
            _gattServer?.RemoveService(nativeService);
        }

        public void RemoveAllServices()
        {
            _gattServer?.ClearServices();
        }

        public bool SendResponse(IAttRequest request, int offset, byte[] value)
        {
            AttRequest attRequest = (AttRequest) request;
            return _gattServer != null && _gattServer.SendResponse(attRequest.BluetoothDevice, attRequest.RequestId, GattStatus.Success, offset, value);
        }

        public bool Notify(IDevice device, ICharacteristic characteristic, byte[] value)
        {
            Device androidDevice = (Device) device;
            Characteristic androidCharacteristic = (Characteristic) characteristic;

            bool indicate = androidCharacteristic.Properties.HasFlag(CharacteristicProperty.Indicate);
            if (!indicate)
            {
                if (!androidCharacteristic.Properties.HasFlag(CharacteristicProperty.Notify))
                {
                    throw new NotSupportedException("Characteristic does not support Notify or Indicate");
                }
            }
            var result = _gattServer.NotifyCharacteristicChanged(androidDevice.NativeDevice, androidCharacteristic.NativeCharacteristic, indicate);
            return result;
        }
    }
}