using System;
using CoreBluetooth;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.iOS.Central
{
    public class Characteristic : ICharacteristic
    {
        private readonly CBCharacteristic _characteristic;

        public Characteristic(CBCharacteristic characteristic)
        {
            _characteristic = characteristic;
        }

        public Guid Uuid => Guid.Parse(_characteristic.UUID.ToString());
    }
}