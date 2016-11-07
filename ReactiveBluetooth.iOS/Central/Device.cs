using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;
using ReactiveBluetooth.Core.Types;
using ReactiveBluetooth.iOS.Extensions;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace ReactiveBluetooth.iOS.Central
{
    public class Device : IDevice
    {
        private readonly PeripheralDelegate.PeripheralDelegate _cbPeripheralDelegate;

        public Device(CBPeripheral peripheral, int rssi, IAdvertisementData advertisementData)
        {
            AdvertisementData = advertisementData;
            Peripheral = peripheral;
            _cbPeripheralDelegate = new PeripheralDelegate.PeripheralDelegate();
            peripheral.Delegate = _cbPeripheralDelegate;
            var currentRssi = Observable.Return(rssi);
            var delegateRssi = _cbPeripheralDelegate.RssiUpdatedSubject.Select(x => x.Item1.RSSI.Int32Value);
            Rssi = currentRssi.Merge(delegateRssi);
        }

        public CBPeripheral Peripheral { get; }
        public Guid Uuid => Guid.Parse(Peripheral.Identifier.ToString());
        public string Name => Peripheral.Name;
        public ConnectionState State => (ConnectionState) Peripheral.State;
        public IAdvertisementData AdvertisementData { get; }
        public IObservable<int> Rssi { get; }

        public Task<IList<IService>> DiscoverServices(CancellationToken cancellationToken)
        {
            if (Peripheral.Services != null)
            {
                return Task.FromResult<IList<IService>>(Peripheral.Services.Select(y => new Service(y, Peripheral, _cbPeripheralDelegate))
                    .Cast<IService>()
                    .ToList());
            }
            return Observable.Create<IList<IService>>(observer =>
            {
                Peripheral.DiscoverServices();
                return Disposable.Empty;
            })
                .Merge(_cbPeripheralDelegate.DiscoveredServicesSubject.Select(x =>
                {
                    return x.Item1.Services.Select(y => new Service(y, Peripheral, _cbPeripheralDelegate))
                        .Cast<IService>()
                        .ToList();
                }))
                .Take(1)
                .ToTask(cancellationToken);
        }

        public void UpdateRemoteRssi()
        {
            Peripheral.ReadRSSI();
        }

        public Task<byte[]> ReadValue(ICharacteristic characteristic, CancellationToken cancellationToken)
        {
            CBCharacteristic cbCharacteristic = ((Characteristic) characteristic).NativeCharacteristic;
            var observable = _cbPeripheralDelegate.UpdatedCharacterteristicValueSubject.FirstAsync(x =>
            {
                bool perphEqual = x.Item1.Identifier.ToString() == Peripheral.Identifier.ToString();
                bool chgarEqual = x.Item2.UUID.Uuid == cbCharacteristic.UUID.Uuid;
                return perphEqual && chgarEqual;
            })
                .Select(x => x.Item2.Value?.ToArray());

            return Observable.Create<byte[]>(observer =>
            {
                Peripheral.ReadValue(cbCharacteristic);
                return Disposable.Empty;
            })
                .Merge(observable)
                .FirstAsync()
                .ToTask(cancellationToken);
        }

        public Task<byte[]> ReadValue(IDescriptor descriptor, CancellationToken cancellationToken)
        {
            CBDescriptor nativeDescriptor = ((Descriptor) descriptor).NativeDescriptor;
            var observable = _cbPeripheralDelegate.UpdatedValueSubject.FirstAsync(x =>
            {
                bool perphEqual = x.Item1.Identifier.ToString() == Peripheral.Identifier.ToString();
                bool descriptorEqual = x.Item2.UUID.Uuid == nativeDescriptor.UUID.Uuid;
                return perphEqual && descriptorEqual;
            })
                .Select(x =>
                {
                    NSData data = (NSData) x.Item2.Value;
                    return data?.ToArray();
                });

            return Observable.Create<byte[]>(observer =>
            {
                Peripheral.ReadValue(nativeDescriptor);
                return Disposable.Empty;
            })
                .Merge(observable)
                .FirstAsync()
                .ToTask(cancellationToken);
        }

        public Task<bool> WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken)
        {
            CBCharacteristic nativeCharacteristic = ((Characteristic) characteristic).NativeCharacteristic;
            var writeObservable = Observable.Create<bool>(observer =>
            {
                Peripheral.WriteValue(NSData.FromArray(value), nativeCharacteristic, writeType.ToCharacteristicWriteType());
                return Disposable.Empty;
            });

            if (writeType == WriteType.WithResponse)
            {
                return writeObservable.Merge<bool>(_cbPeripheralDelegate.WroteCharacteristicValueSubject.FirstAsync(x =>
                {
                    bool perphEqual = x.Item1.Identifier.ToString() == Peripheral.Identifier.ToString();
                    bool chgarEqual = x.Item2.UUID.Uuid == nativeCharacteristic.UUID.Uuid;
                    return perphEqual && chgarEqual;
                })
                    .Select(x =>
                    {
                        if (x.Item3 != null)
                        {
                            throw new Exception(x.Item3.LocalizedDescription);
                        }
                        return true;
                    }))
                    .ToTask(cancellationToken);
            }

            return writeObservable.Merge<bool>(Observable.Return(true))
                .ToTask(cancellationToken);
        }

        public Task<bool> WriteValue(IDescriptor descriptor, byte[] value, CancellationToken cancellationToken)
        {
            CBDescriptor nativeDescriptor = ((Descriptor) descriptor).NativeDescriptor;
            var writeObservable = Observable.Create<bool>(observer =>
            {
                Peripheral.WriteValue(NSData.FromArray(value), nativeDescriptor);
                return Disposable.Empty;
            });

            return writeObservable.Merge(_cbPeripheralDelegate.WroteDescriptorValueSubject.FirstAsync(x =>
            {
                bool perphEqual = x.Item1.Identifier.ToString() == Peripheral.Identifier.ToString();
                bool desciptorEqual = x.Item2.UUID.Uuid == nativeDescriptor.UUID.Uuid;
                return perphEqual && desciptorEqual;
            })
                .Select(tuple =>
                {
                    if (tuple.Item3 != null)
                    {
                        throw new Exception(tuple.Item3.LocalizedDescription);
                    }
                    return true;
                }))
                .ToTask(cancellationToken);
        }

        public IObservable<byte[]> Notifications(ICharacteristic characteristic)
        {
            CBCharacteristic nativeCharacteristic = ((Characteristic) characteristic).NativeCharacteristic;
            Subject<byte[]> s = new Subject<byte[]>();
            IObservable<byte[]> enableNotificationObservable = Observable.Create<byte[]>(observer =>
            {
                Peripheral.SetNotifyValue(true, nativeCharacteristic);
                return Disposable.Create(() => { Peripheral.SetNotifyValue(false, nativeCharacteristic); });
            })
                .Publish()
                .RefCount();

            var valueUpdatedObservable = _cbPeripheralDelegate.UpdatedCharacterteristicValueSubject.Select(x => x.Item2.Value.ToArray());
            var stateUpdatedObservable = _cbPeripheralDelegate.UpdatedNotificationStateSubject.Select(x =>
            {
                if (x.Item3 != null)
                {
                    throw new NotificationException(x.Item3.LocalizedDescription);
                }
                return x.Item2.Value?.ToArray();
            });
            return enableNotificationObservable.Merge(valueUpdatedObservable)
                .Merge(stateUpdatedObservable);
        }

        public bool RequestConnectionPriority(ConnectionPriority priority)
        {
            return true;
        }
    }
}