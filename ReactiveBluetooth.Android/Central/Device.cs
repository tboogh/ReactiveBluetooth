using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.Android.Central
{
    public class Device : IDevice
    {
        public Device(BluetoothDevice device)
        {
            NativeDevice = device;
        }

        public BluetoothDevice NativeDevice { get; }
        public BluetoothGatt Gatt { get; set; }
        public string Name => NativeDevice.Name;

        public Guid Uuid
        {
            get
            {
                Byte[] deviceGuid = new Byte[16];
                String macWithoutColons = NativeDevice.Address.Replace(":", "");
                Byte[] macBytes = Enumerable.Range(0, macWithoutColons.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(macWithoutColons.Substring(x, 2), 16))
                    .ToArray();
                macBytes.CopyTo(deviceGuid, 10);
                return new Guid(deviceGuid);
            }
        }
        public BleGattCallback Callback { get; set; }

        public ConnectionState State
        {
            get
            {
                var manager = (BluetoothManager) Application.Context.GetSystemService(Context.BluetoothService);
                return (ConnectionState) manager.GetConnectionState(NativeDevice, ProfileType.Gatt);
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