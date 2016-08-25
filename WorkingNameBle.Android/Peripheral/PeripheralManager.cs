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

namespace WorkingNameBle.Android.Peripheral
{
    public class PeripheralManager : IPeripheralManager
    {
        private BluetoothAdapter _bluetoothAdapter;
        private BluetoothLeAdvertiser _bluetoothLeAdvertiser;
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
            
        }

        public IObservable<bool> StartAdvertising(AdvertisingOptions advertisingOptions)
        {
            if (advertisingOptions.LocalName != null)
            {
                _bluetoothAdapter.SetName(advertisingOptions.LocalName);
            }
            

            AdvertiseSettings.Builder settingsBuilder = new AdvertiseSettings.Builder();
            settingsBuilder.SetAdvertiseMode(AdvertiseMode.Balanced);
            settingsBuilder.SetConnectable(true);
            settingsBuilder.SetTxPowerLevel(AdvertiseTx.PowerMedium);
            settingsBuilder.SetTimeout(5000);
            

            var parcelUuids = advertisingOptions.ServiceUuids.Select(x => ParcelUuid.FromString(x.ToString()));
            var dataBuilder = new AdvertiseData.Builder();

            foreach (var parcelUuid in parcelUuids)
            {
                dataBuilder.AddServiceUuid(parcelUuid);
            }
            dataBuilder.SetIncludeDeviceName(advertisingOptions.LocalName != null);

            var settings = settingsBuilder.Build();
            var advertiseData = dataBuilder.Build();

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
    }
}