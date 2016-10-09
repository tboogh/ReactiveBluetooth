using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Bluetooth;
using Java.Util;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Android.Peripheral.GattServer;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property, IServerCallback serverCallback)
        {
            var nativePermissions = permission.ToGattPermission();
            var nativeProperties = property.ToGattProperty();

            var characteristic = new BluetoothGattCharacteristic(UUID.FromString(uuid.ToString()), nativeProperties, nativePermissions);
            if (!characteristic.SetValue(value))
            {
                throw new Exception("Failed to set characteristic value");
            }
            NativeCharacteristic = characteristic;

            ReadRequestObservable = serverCallback.CharacteristicReadRequestSubject
                .Where(x => x.Item4.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Item3, null, request.Item2, request.Item1));

            WriteRequestObservable = serverCallback.CharacteristicWriteRequestSubject
                .Where(x => x.Item3.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Item6, request.Item7, request.Item2, request.Item1));
        }

        public BluetoothGattCharacteristic NativeCharacteristic { get; }
        public Guid Uuid => Guid.Parse(NativeCharacteristic.Uuid.ToString());
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty();
        public IDescriptor[] Descriptors => NativeCharacteristic.Descriptors?.Select(x => new Descriptor(x))
           .Cast<IDescriptor>()
           .ToArray();

        public CharacteristicPermission Permissions => NativeCharacteristic.Permissions.ToCharacteristicPermission();
        public void AddDescriptor(IDescriptor descriptor)
        {
            var nativeDescriptor = ((Descriptor) descriptor).NativeDescriptor;
            NativeCharacteristic.AddDescriptor(nativeDescriptor);
        }

        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }
    }
}