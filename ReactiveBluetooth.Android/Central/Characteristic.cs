using System;
using Android.Bluetooth;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.Android.Central
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