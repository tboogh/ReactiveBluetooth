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
            GattCharacteristic = characteristic;
        }

        public BluetoothGattCharacteristic GattCharacteristic { get; }

        public Guid Uuid => Guid.Parse(GattCharacteristic.Uuid.ToString());
        public CharacteristicProperty Properties => GattCharacteristic.Properties.ToCharacteristicProperty();

        public IDescriptor[] Descriptors => GattCharacteristic.Descriptors.Select(x => new Descriptor(x))
            .Cast<IDescriptor>()
            .ToArray();
    }
} 