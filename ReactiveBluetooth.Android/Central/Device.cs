using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS.Storage;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;
using IService = ReactiveBluetooth.Core.Central.IService;

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

        public Task<IList<IService>> DiscoverServices(CancellationToken cancellationToken)
        {
            return Observable.FromEvent<IList<IService>>(action => { Gatt.DiscoverServices(); }, action => { })
                .Merge(GattCallback.ServicesDiscovered.Select(x =>
                {
                    return Gatt.Services.Select(bluetoothGattService => new Service(bluetoothGattService))
                        .Cast<IService>()
                        .ToList();
                }))
                .Take(1)
                .ToTask(cancellationToken);
        }

        public void UpdateRemoteRssi()
        {
            Gatt.ReadRemoteRssi();
        }

        public Task<byte[]> ReadValue(ICharacteristic characteristic, CancellationToken cancellationToken)
        {
            BluetoothGattCharacteristic gattCharacteristic = ((Characteristic) characteristic).GattCharacteristic;
            var observable = GattCallback.CharacteristicReadSubject.FirstAsync(x => x.Item2 == gattCharacteristic)
                .Select(x => x.Item2.GetValue());
            return Observable.FromEvent<byte[]>(action => Gatt.ReadCharacteristic(gattCharacteristic), _ => { })
                .Merge(observable)
                .FirstAsync()
                .ToTask(cancellationToken);
        }

        public Task<bool> WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken)
        {
            BluetoothGattCharacteristic gattCharacteristic = ((Characteristic)characteristic).GattCharacteristic;

            var writeObservable = Observable.FromEvent<bool>(action =>
            {
                gattCharacteristic.WriteType = writeType.ToGattWriteType();
                gattCharacteristic.SetValue(value);

                Gatt.WriteCharacteristic(gattCharacteristic);
            }, _ => { });

            if (writeType == WriteType.WithResponse)
            {
                return writeObservable.Merge<bool>(GattCallback.CharacteristicWriteSubject.FirstAsync(x => x.Item2 == gattCharacteristic).Select(
                    x =>
                    {
                        if (x.Item3 != GattStatus.Success)
                        {
                            throw new Exception($"Failed to write characteristic: {x.Item3.ToString()}");
                        }
                        return true;
                    })).Take(1).ToTask(cancellationToken);
            }
            return writeObservable.Merge(Observable.Return(true)).Take(1).ToTask(cancellationToken);
        }
    }
}