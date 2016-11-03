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
using Java.Util;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Types;
using IService = ReactiveBluetooth.Core.Central.IService;
using Observable = System.Reactive.Linq.Observable;

namespace ReactiveBluetooth.Android.Central
{
    public class Device : IDevice
    {
        public Device(BluetoothDevice device, int rssi, AdvertisementData advertisementData)
        {
            AdvertisementData = advertisementData;
            NativeDevice = device;
            GattCallback = new GattCallback();
            var currentRssi = Observable.Return(rssi);
            var callbackRssi = GattCallback.ReadRemoteRssiSubject.Select(x => x.Item2);
            Rssi = currentRssi.Merge(callbackRssi);
        }

        public BluetoothDevice NativeDevice { get; }
        public BluetoothGatt Gatt { get; set; }
        public GattCallback GattCallback { get; }
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

        public IAdvertisementData AdvertisementData { get; }
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
            BluetoothGattCharacteristic gattCharacteristic = ((Characteristic) characteristic).NativeCharacteristic;
            var observable = GattCallback.CharacteristicReadSubject.FirstAsync(x => x.Item2 == gattCharacteristic)
                .Select(x => x.Item2.GetValue());
            
            return Observable.FromEvent<byte[]>(action => Gatt.ReadCharacteristic(gattCharacteristic), _ => { })
                .Merge(observable)
                .FirstAsync()
                .ToTask(cancellationToken);
        }

        /// <summary>
        /// Reads a value from a descriptor
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<byte[]> ReadValue(IDescriptor descriptor, CancellationToken cancellationToken)
        {
            BluetoothGattDescriptor nativeDescriptor = ((Descriptor)descriptor).NativeDescriptor;
            var observable = GattCallback.DescriptorReadSubject.FirstAsync(x => x.Item2 == nativeDescriptor)
                .Select(x => x.Item2.GetValue());

            return Observable.FromEvent<byte[]>(action => Gatt.ReadDescriptor(nativeDescriptor), _ => { })
                .Merge(observable)
                .FirstAsync()
                .ToTask(cancellationToken);
        }

        /// <summary>
        /// Writes a value to a characterstic
        /// </summary>
        /// <param name="characteristic">The characterstic whose value is to be written</param>
        /// <param name="value">The value to be written</param>
        /// <param name="writeType">The type of write to be executed. See <see cref="WriteType"/> for possible types. Defaults to <see cref="WriteType.WithoutRespoonse"/></param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel the write operation</param>
        /// <returns>Task with result of write operation</returns>
        public Task<bool> WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken)
        {
            BluetoothGattCharacteristic gattCharacteristic = ((Characteristic) characteristic).NativeCharacteristic;

            var writeObservable = Observable.FromEvent<bool>(action =>
            {
                gattCharacteristic.WriteType = writeType.ToGattWriteType();

                var setValueResult = gattCharacteristic.SetValue(value);
                if (!setValueResult)
                    action(false);

                var result = Gatt.WriteCharacteristic(gattCharacteristic);
                if (!result)
                    action(false);
            }, _ => { });

            if (writeType == WriteType.WithResponse)
            {
                return writeObservable.Merge<bool>(GattCallback.CharacteristicWriteSubject.FirstAsync(x => x.Item2 == gattCharacteristic)
                    .Select(x =>
                    {
                        if (x.Item3 != GattStatus.Success)
                        {
                            throw new Exception($"Failed to write characteristic: {x.Item3.ToString()}");
                        }
                        return true;
                    }))
                    .Take(1)
                    .ToTask(cancellationToken);
            }
            return writeObservable.Merge(Observable.Return(true))
                .Take(1)
                .ToTask(cancellationToken);
        }

        /// <summary>
        ///  Writes a value to a descriptor
        /// </summary>
        /// <param name="descriptor">The descriptor whose value is to be written</param>
        /// <param name="value">The value that is to be written</param>
        /// <param name="cancellationToken">The cancellation token that can be used to cancel the write operation</param>
        /// <returns>Task with result of write operation</returns>
        public Task<bool> WriteValue(IDescriptor descriptor, byte[] value, CancellationToken cancellationToken)
        {
            BluetoothGattDescriptor gattDescriptor = ((Descriptor) descriptor).NativeDescriptor;

            var writeObservable = Observable.FromEvent<bool>(action =>
            {
                var result = gattDescriptor.SetValue(value);
                if (!result)
                    action(false);

                var writeResult = Gatt.WriteDescriptor(gattDescriptor);
                if (!writeResult)
                    action(false);
            }, _ => { });

            return writeObservable.Merge(GattCallback.DescriptorWriteSubject.FirstAsync(x => x.Item2 == gattDescriptor)
                .Select(x =>
                {
                    if (x.Item3 != GattStatus.Success)
                    {
                        throw new Exception($"Failed to write desciptor: {x.Item3.ToString()}");
                    }
                    return true;
                }))
                .Take(1)
                .ToTask(cancellationToken);
        }

        /// <summary>
        /// Subscribing to this will enable notifications on the <param name="characteristic">characteristic</param>. When it is disposed notifications will be disabled on the characterstic.
        /// If the characteristic supports both <see cref="CharacteristicProperty.Notify"/> and <see cref="CharacteristicProperty.Indicate"/> it will use <see cref="CharacteristicProperty.Notify"/>
        /// </summary>
        /// <param name="characteristic">The characteristic to enable notifications on</param>
        /// <returns>Observable that is updated with characteristic values</returns>
        /// <exception cref="NotificationException">Thrown if characteristic does not support notifiying or indicating, also thrown if operation fails</exception>
        public IObservable<byte[]> Notifications(ICharacteristic characteristic)
        {
            BluetoothGattCharacteristic nativeCharacteristic = ((Characteristic)characteristic).NativeCharacteristic;

            IObservable<byte[]> notificationObservable = Observable.FromEvent<byte[]>(async action =>
            {
                IList<byte> enableNotificationValue = null;
                if (characteristic.Properties.HasFlag(CharacteristicProperty.Notify))
                {
                    enableNotificationValue = BluetoothGattDescriptor.EnableNotificationValue;
                } else if (characteristic.Properties.HasFlag(CharacteristicProperty.Indicate))
                {
                    enableNotificationValue = BluetoothGattDescriptor.EnableIndicationValue;
                }
                if (enableNotificationValue == null)
                {
                    throw new NotificationException("Characteristic does not support notifications");
                }

                var uuid = "2902".ToGuid();

                var characteristicConfigDescriptor = characteristic.Descriptors.FirstOrDefault(x => x.Uuid == uuid);
                if (characteristicConfigDescriptor == null)
                    return;

                CancellationTokenSource cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                try
                {
                    await WriteValue(characteristicConfigDescriptor, enableNotificationValue.ToArray(), cancellationToken.Token);
                }
                catch (TaskCanceledException)
                {
                    throw new NotificationException("SetCharacteristicNotification enable failed");
                }
                if (!Gatt.SetCharacteristicNotification(nativeCharacteristic, true))
                {
                    throw new NotificationException("SetCharacteristicNotification enable failed");
                }
            }, action =>
            {
                if (!Gatt.SetCharacteristicNotification(nativeCharacteristic, false))
                {
                    throw new NotificationException("SetCharacteristicNotification disable failed");
                }

                var characteristicConfigDescriptor = nativeCharacteristic.GetDescriptor(UUID.FromString("2902".ToGuid()
                    .ToString()));
                characteristicConfigDescriptor.SetValue(BluetoothGattDescriptor.DisableNotificationValue.ToArray());
                if (!Gatt.WriteDescriptor(characteristicConfigDescriptor))
                {
                    throw new NotificationException("WriteDescriptor disable failed");
                }
            });

            
            return notificationObservable.Merge(GattCallback.CharacteristicChangedSubject.Select(x => x.Item2.GetValue()));
        }
    }
}