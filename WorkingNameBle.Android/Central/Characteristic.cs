using System;
using Android.Bluetooth;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Android.Central
{
    public class Characteristic : ICharacteristic
    {
        private readonly BluetoothGattCharacteristic _characteristic;

        public Characteristic(BluetoothGattCharacteristic characteristic)
        {
            _characteristic = characteristic;
        }

        public Guid Id => Guid.Parse(_characteristic.Uuid.ToString());
    }
} 