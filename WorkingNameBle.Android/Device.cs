using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkingNameBle.Core;

namespace WorkingNameBle.Android
{
    public class Device : IDevice
    {
        public Device(BluetoothDevice device)
        {
            NativeDevice = device;
            Name = device.Name;
        }

        public BluetoothDevice NativeDevice { get; private set; }
        public BluetoothGatt Gatt { get; set; }
        public string Name { get; }
        public BleGattCallback Callback { get; set; }

        public ConnectionState State
        {
            get
            {
                var manager = (BluetoothManager) Application.Context.GetSystemService(Context.BluetoothService);
                return (ConnectionState) manager.GetConnectionState(NativeDevice, ProfileType.Gatt);
                ;
            }
        }

        public IObservable<IList<IService>> DiscoverServices()
        {
            var discoverObservable = Observable.Create<IList<IService>>(observer =>
            {
                Callback.ServicesDiscovered = (gatt, status) =>
                {
                    IList<IService> services = Gatt.Services.Select(bluetoothGattService => new Service(bluetoothGattService))
                        .Cast<IService>()
                        .ToList();

                    observer.OnNext(services);
                    observer.OnCompleted();
                };

                Gatt.DiscoverServices();

                return Disposable.Create(() => { Callback.ServicesDiscovered = null; });
            });
            return discoverObservable;
        }
    }
}