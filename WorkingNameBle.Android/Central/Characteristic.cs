using System;
using Android.Bluetooth;
using WorkingNameBle.Android.Extensions;
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

        public Guid Uuid => Guid.Parse(_characteristic.Uuid.ToString());
        public CharacteristicProperty Properties => _characteristic.Properties.ToCharacteristicProperty();
        public CharacteristicPermission Permissions => _characteristic.Permissions.ToCharacteristicPermission();
    }
} 