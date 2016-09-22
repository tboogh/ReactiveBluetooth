using System;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.iOS.Extensions;

namespace ReactiveBluetooth.iOS.Central
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(CBCharacteristic characteristic)
        {
            NativeCharacteristic = characteristic;
        }

        public CBCharacteristic NativeCharacteristic { get; }

        public Guid Uuid => Guid.Parse(NativeCharacteristic.UUID.ToString());
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty(); 
    }
}