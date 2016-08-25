using System;
using CoreBluetooth;
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

        public Guid Id => Guid.Parse(_characteristic.UUID.ToString());
    }
}