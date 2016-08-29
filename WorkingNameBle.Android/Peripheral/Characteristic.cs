using System;
using Android.Bluetooth;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.Android.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(BluetoothGattCharacteristic characteristic)
        {
            GattCharacteristic = characteristic;
        }

        public BluetoothGattCharacteristic GattCharacteristic { get; }
        public Guid Uuid { get; }
    }
}