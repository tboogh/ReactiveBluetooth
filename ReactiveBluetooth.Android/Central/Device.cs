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
        public Device(BluetoothDevice device, int rssi)
        {
            NativeDevice = device;
            GattCallback = new BleGattCallback();
            var currentRssi = Observable.Return(rssi);
            var callbackRssi = GattCallback.ReadRemoteRssiSubject.Select(x => x.Item2);

            Rssi = currentRssi.Merge(callbackRssi);
        }

        public BluetoothDevice NativeDevice { get; }
        public BluetoothGatt Gatt { get; set; }
        public BleGattCallback GattCallback { get; }
        public string Name => NativeDevice.Name;

        public Guid Uuid
        {
            get
            {
                Byte[] deviceGuid = new Byte[16];
                String macWithoutColons = NativeDevice.Address.Replace(":", "");
                Byte[] macBytes = Enumerable.Range(0, macWithoutColons.Length)
                    .Where(x => x%2 == 0)
                    .Select(x => Convert.ToByte(macWithoutColons.Substring(x, 2), 16))
                    .ToArray();
                macBytes.CopyTo(deviceGuid, 10);
                return new Guid(deviceGuid);
            }
        }

        public ConnectionState State
        {
            get
            {
                var manager = (BluetoothManager) Application.Context.GetSystemService(Context.BluetoothService);
                return (ConnectionState) manager.GetConnectionState(NativeDevice, ProfileType.Gatt);
            }
        }

        public IObservable<int> Rssi { get; }

        public IObservable<IList<IService>> DiscoverServices()
        {
            return Observable.Create<IList<IService>>(observer =>
            {
                var discoverDisposable = GattCallback.ServicesDiscovered.Select(x =>
                {
                    IList<IService> services = Gatt.Services.Select(bluetoothGattService => new Service(bluetoothGattService))
                        .Cast<IService>()
                        .ToList();
                    return services;
                }).Subscribe(observer.OnNext);

                Gatt.DiscoverServices();
                return Disposable.Create(() =>
                {
                    discoverDisposable.Dispose();
                });
            });
        }

        public void UpdateRemoteRssi()
        {
            Gatt.ReadRemoteRssi();
        }
    }
}