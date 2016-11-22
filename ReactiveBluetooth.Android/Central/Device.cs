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
        private IDisposable _connectionStateDisposable;

        public Device(BluetoothDevice device, int rssi, AdvertisementData advertisementData)
        {
            AdvertisementData = advertisementData;
            NativeDevice = device;
            GattCallback = new GattCallback();
            var currentRssi = Observable.Return(rssi);
            var callbackRssi = GattCallback.ReadRemoteRssiSubject.Select(x => x.Item2);
            Rssi = currentRssi.Merge(callbackRssi);

            _connectionStateDisposable = GattCallback.ConnectionStateChange.Subscribe(state =>
            { State = (ConnectionState) state; });
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

        public ConnectionState State { get; private set; }

        public IAdvertisementData AdvertisementData { get; }
        public IObservable<int> Rssi { get; }

        public Task<IList<IService>> DiscoverServices(CancellationToken cancellationToken)
        {
            return Observable.Create<IList<IService>>(observer =>
            {
                Gatt.DiscoverServices();
                return Disposable.Empty;
            })
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
            
            return Observable.Create<byte[]>(observer =>
            {
                bool read = Gatt.ReadCharacteristic(gattCharacteristic);
                if (!read)
                    observer.OnError(new Exception("Failed to read characteristic"));

                return Disposable.Empty;
            })
                .Merge(observable)
                .FirstAsync()
                .ToTask(cancellationToken);
        }

        public Task<byte[]> ReadValue(IDescriptor descriptor, CancellationToken cancellationToken)
        {
            BluetoothGattDescriptor nativeDescriptor = ((Descriptor)descriptor).NativeDescriptor;
            var observable = GattCallback.DescriptorReadSubject.FirstAsync(x => x.Item2 == nativeDescriptor)
                .Select(x => x.Item2.GetValue());

            return Observable.Create<byte[]>(observer =>
            {
                Gatt.ReadDescriptor(nativeDescriptor);
                return Disposable.Empty;
            })
                .Merge(observable)
                .FirstAsync()
                .ToTask(cancellationToken);
        }

        public Task<bool> WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken)
        {
            BluetoothGattCharacteristic gattCharacteristic = ((Characteristic) characteristic).NativeCharacteristic;

            var writeObservable = Observable.Create<bool>(observer =>
            {
                gattCharacteristic.WriteType = writeType.ToGattWriteType();

                var setValueResult = gattCharacteristic.SetValue(value);
                if (!setValueResult)
                    observer.OnError(new Exception("Failed to set value"));

                var result = Gatt.WriteCharacteristic(gattCharacteristic);
                if (!result)
                    observer.OnError(new Exception("Failed to write characteristic"));

                return Disposable.Empty;
            });

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

        public Task<bool> WriteValue(IDescriptor descriptor, byte[] value, CancellationToken cancellationToken)
        {
            BluetoothGattDescriptor gattDescriptor = ((Descriptor) descriptor).NativeDescriptor;

            var writeObservable = Observable.Create<bool>(observer =>
            {
                if (Gatt == null)
                {
                    observer.OnError(new Exception("Not connected"));
                    return Disposable.Empty;
                }

                var result = gattDescriptor.SetValue(value);
                if (!result)
                    observer.OnError(new Exception("Failed to set value"));

                var writeResult = Gatt.WriteDescriptor(gattDescriptor);
                if (!writeResult)
                    observer.OnError(new Exception("Failed to write descriptor"));

                return Disposable.Empty;
            });

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

        public IObservable<byte[]> Notifications(ICharacteristic characteristic)
        {
            BluetoothGattCharacteristic nativeCharacteristic = ((Characteristic)characteristic).NativeCharacteristic;

            IObservable<byte[]> notificationObservable = Observable.Create<byte[]>(async observer =>
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
                    observer.OnError(new NotificationException("Characteristic does not support notifications"));
                    return Disposable.Empty;
                }

                var uuid = "2902".ToGuid();

                var characteristicConfigDescriptor = characteristic.Descriptors.FirstOrDefault(x => x.Uuid == uuid);
                if (characteristicConfigDescriptor == null)
                {
                    observer.OnError(new NotificationException("Missing client configuration descriptor"));
                    return Disposable.Empty;
                }

                try
                {
                    CancellationTokenSource timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                    var writeResult = await WriteValue(characteristicConfigDescriptor, enableNotificationValue.ToArray(), timeoutSource.Token);
                    if (!writeResult)
                    {
                        observer.OnError(new NotificationException("Failed to write descriptor"));
                        return Disposable.Empty;
                    }
                }
                catch (Exception exception)
                {
                    observer.OnError(new NotificationException("Failed to write descriptor"));
                    return Disposable.Empty;
                }
                if (!Gatt.SetCharacteristicNotification(nativeCharacteristic, true))
                {
                    observer.OnError(new NotificationException("SetCharacteristicNotification enable failed"));
                    return Disposable.Empty;
                }

                return Disposable.Create(async () =>
                {
                    try
                    {
                        CancellationTokenSource timeoutSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                        await WriteValue(characteristicConfigDescriptor, BluetoothGattDescriptor.DisableNotificationValue.ToArray(), timeoutSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        throw new NotificationException("Failed to write descriptor");
                    }
                    if (!Gatt.SetCharacteristicNotification(nativeCharacteristic, false))
                    {
                        throw new NotificationException("SetCharacteristicNotification disable failed");
                    }
                });
            });


            return notificationObservable.Publish().RefCount().Merge(GattCallback.CharacteristicChangedSubject.Select(x => x.Item2.GetValue()));
        }

        public bool RequestConnectionPriority(ConnectionPriority priority)
        {
            return Gatt.RequestConnectionPriority(priority.ToConnectionPriority());
        }

        public void Dispose()
        {
            _connectionStateDisposable?.Dispose();
        }
    }
}