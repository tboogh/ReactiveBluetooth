using System;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.iOS.Central
{
    public class Characteristic : ICharacteristic
    {
        private readonly CBCharacteristic _characteristic;

        public Characteristic(CBCharacteristic characteristic)
        {
            _characteristic = characteristic;
        }

        public Guid Uuid => Guid.Parse(_characteristic.UUID.ToString());
        public CharacteristicProperty Properties { get; }
        public CharacteristicPermission Permissions { get; }
    }
}