using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.iOS.Extensions
{
    public static class CharacteristicPropertyExtensions
    {
        public static CBCharacteristicProperties ToCBCharacteristicProperty(this CharacteristicProperty property)
        {
            CBCharacteristicProperties nativeProperties = 0;

            if (property.HasFlag(CharacteristicProperty.Broadcast))
            {
                nativeProperties |= CBCharacteristicProperties.Broadcast;
            }
            if (property.HasFlag(CharacteristicProperty.Read))
            {
                nativeProperties |= CBCharacteristicProperties.Read;
            }
            if (property.HasFlag(CharacteristicProperty.WriteWithoutResponse))
            {
                nativeProperties |= CBCharacteristicProperties.WriteWithoutResponse;
            }
            if (property.HasFlag(CharacteristicProperty.Write))
            {
                nativeProperties |= CBCharacteristicProperties.Write;
            }
            if (property.HasFlag(CharacteristicProperty.Notify))
            {
                nativeProperties |= CBCharacteristicProperties.Notify;
            }
            if (property.HasFlag(CharacteristicProperty.Indicate))
            {
                nativeProperties |= CBCharacteristicProperties.Indicate;
            }
            if (property.HasFlag(CharacteristicProperty.AuthenticatedSignedWrites))
            {
                nativeProperties |= CBCharacteristicProperties.AuthenticatedSignedWrites;
            }
            if (property.HasFlag(CharacteristicProperty.ExtendedProperties))
            {
                nativeProperties |= CBCharacteristicProperties.ExtendedProperties;
            }

            return nativeProperties;
        }
    }
}
