using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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
                .Select(x => State);

        }

        public void Shutdown()
        {
            
        }

        public void StartAdvertising(AdvertisingOptions advertisingOptions)
        {
            throw new NotImplementedException();
        }
    }
}