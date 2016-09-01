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
    public static class GattPropertyExtensions
    {
        public static CharacteristicProperty ToCharacteristicProperty(this GattProperty nativeProperty)
        {
            CharacteristicProperty properties = 0;
            if (nativeProperty.HasFlag(GattProperty.Broadcast))
            {
                properties |= CharacteristicProperty.Broadcast;
            }
            if (nativeProperty.HasFlag(GattProperty.Read))
            {
                properties |= CharacteristicProperty.Read;
            }
            if (nativeProperty.HasFlag(GattProperty.WriteNoResponse))
            {
                properties |= CharacteristicProperty.WriteWithoutResponse;
            }
            if (nativeProperty.HasFlag(GattProperty.Write))
            {
                properties |= CharacteristicProperty.Write;
            }
            if (nativeProperty.HasFlag(GattProperty.Notify))
            {
                properties |= CharacteristicProperty.Notify;
            }
            if (nativeProperty.HasFlag(GattProperty.Indicate))
            {
                properties |= CharacteristicProperty.Indicate;
            }
            if (nativeProperty.HasFlag(GattProperty.SignedWrite))
            {
                properties |= CharacteristicProperty.AuthenticatedSignedWrites;
            }
            if (nativeProperty.HasFlag(GattProperty.ExtendedProps))
            {
                properties |= CharacteristicProperty.ExtendedProperties;
            }
            return properties;
        }
    }
}