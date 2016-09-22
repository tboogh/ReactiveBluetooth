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

        public Guid Uuid => Guid.Parse(NativeCharacteristic.UUID.Uuid);
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty(); 
    }
}