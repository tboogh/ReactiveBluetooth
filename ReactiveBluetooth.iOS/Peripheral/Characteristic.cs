﻿using System;
using System.Linq;
using System.Reactive.Linq;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using ReactiveBluetooth.iOS.Extensions;
using ReactiveBluetooth.iOS.Peripheral.PeripheralManagerDelegate;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property, PeripheralManagerDelegate.PeripheralManagerDelegate peripheralDelegate)
        {
            var nativePermissions = permission.ToCBAttributePermission();
            var nativeProperties = property.ToCBCharacteristicProperty();

            NSData nsData = null;
            if (value != null)
            {
                NSData.FromArray(value);
            }

            CBMutableCharacteristic mutableCharacteristic = new CBMutableCharacteristic(CBUUID.FromString(uuid.ToString()), nativeProperties, nsData, nativePermissions);
            NativeCharacteristic = mutableCharacteristic;

            ReadRequestObservable = peripheralDelegate.ReadRequestReceivedSubject.Where(x => x.AttRequest.Characteristic.UUID == NativeCharacteristic.UUID).Select(x => new AttRequest(this, x.AttRequest)).AsObservable();
            WriteRequestObservable = peripheralDelegate.WriteRequestsReceivedSubject.SelectMany(received => received.Requests.Select(x => new AttRequest(this, x)).ToObservable()).Where(x => x.Characteristic.Uuid == Uuid).AsObservable();
        }

        public Guid Uuid => Guid.Parse(NativeCharacteristic.UUID.ToString());
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty();

        public IDescriptor[] Descriptors => NativeCharacteristic.Descriptors.Select(x => new Descriptor(x))
           .Cast<IDescriptor>()
           .ToArray();

        public CharacteristicPermission Permissions => NativeCharacteristic.Permissions.ToCharacteristicPermission();
        public CBMutableCharacteristic NativeCharacteristic { get; }
        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }

        public void AddDescriptor(IDescriptor descriptor)
        {
            var nativeDescriptor = ((Descriptor)descriptor).NativeDescriptor;
            var descriptors = NativeCharacteristic.Descriptors.ToList();
            descriptors.Add(nativeDescriptor);
            NativeCharacteristic.Descriptors = descriptors.ToArray();
        }

        public void Dispose()
        {
            
        }
    }
}