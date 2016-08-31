using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS.Extensions
{
    public static class CBCharacteristicPropertiesExtensions
    {
        public static CharacteristicProperty ToCharacteristicProperty(this CBCharacteristicProperties nativeProperties)
        {
            CharacteristicProperty property = 0;

            if (nativeProperties.HasFlag(CBCharacteristicProperties.Broadcast))
            {
                property |= CharacteristicProperty.Broadcast;
            }
            if (nativeProperties.HasFlag(CBCharacteristicProperties.Read))
            {
                property |= CharacteristicProperty.Read;
            }
            if (nativeProperties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse))
            {
                property |= CharacteristicProperty.WriteWithoutResponse;
            }
            if (nativeProperties.HasFlag(CBCharacteristicProperties.Write))
            {
                property |= CharacteristicProperty.Write;
            }
            if (nativeProperties.HasFlag(CBCharacteristicProperties.Notify))
            {
                property |= CharacteristicProperty.Notify;
            }
            if (nativeProperties.HasFlag(CBCharacteristicProperties.Indicate))
            {
                property |= CharacteristicProperty.Indicate;
            }
            if (nativeProperties.HasFlag(CBCharacteristicProperties.AuthenticatedSignedWrites))
            {
                property |= CharacteristicProperty.AuthenticatedSignedWrites;
            }
            if (nativeProperties.HasFlag(CBCharacteristicProperties.ExtendedProperties))
            {
                property |= CharacteristicProperty.ExtendedProperties;
            }

            return property;
        }
    }
}
