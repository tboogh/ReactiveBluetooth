using System;
using System.Linq;
using Android.Bluetooth;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Central
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(BluetoothGattCharacteristic characteristic)
        {
            NativeCharacteristic = characteristic;
        }

        public BluetoothGattCharacteristic NativeCharacteristic { get; }

        public Guid Uuid => Guid.Parse(NativeCharacteristic.Uuid.ToString());
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty();

        public IDescriptor[] Descriptors => NativeCharacteristic.Descriptors.Select(x => new Descriptor(x))
            .Cast<IDescriptor>()
            .ToArray();
    }
} 