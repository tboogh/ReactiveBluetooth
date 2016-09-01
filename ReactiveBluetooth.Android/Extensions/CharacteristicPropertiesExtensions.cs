using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveBluetooth.Core;

namespace ReactiveBluetooth.Android.Extensions
{
    public static class CharacteristicPropertiesExtensions
    {
        public static GattProperty ToGattProperty(this CharacteristicProperty property)
        {
            GattProperty nativeProperties = 0;
            if (property.HasFlag(CharacteristicProperty.Broadcast))
            {
                nativeProperties |= GattProperty.Broadcast;
            }
            if (property.HasFlag(CharacteristicProperty.Read))
            {
                nativeProperties |= GattProperty.Read;
            }
            if (property.HasFlag(CharacteristicProperty.WriteWithoutResponse))
            {
                nativeProperties |= GattProperty.WriteNoResponse;
            }
            if (property.HasFlag(CharacteristicProperty.Write))
            {
                nativeProperties |= GattProperty.Write;
            }
            if (property.HasFlag(CharacteristicProperty.Notify))
            {
                nativeProperties |= GattProperty.Notify;
            }
            if (property.HasFlag(CharacteristicProperty.Indicate))
            {
                nativeProperties |= GattProperty.Indicate;
            }
            if (property.HasFlag(CharacteristicProperty.AuthenticatedSignedWrites))
            {
                nativeProperties |= GattProperty.SignedWrite;
            }
            if (property.HasFlag(CharacteristicProperty.ExtendedProperties))
            {
                nativeProperties |= GattProperty.ExtendedProps;
            }
            return nativeProperties;
        }
    }
}