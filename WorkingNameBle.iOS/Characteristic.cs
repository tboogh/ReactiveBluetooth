using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS
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