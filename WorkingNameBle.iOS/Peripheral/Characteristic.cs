using System;
using CoreBluetooth;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.iOS.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(CBMutableCharacteristic mutableCharacteristic)
        {
            MutableCharacteristic = mutableCharacteristic;
        }

        public Guid Uuid { get; }
        public CBMutableCharacteristic MutableCharacteristic { get; }
    }
}