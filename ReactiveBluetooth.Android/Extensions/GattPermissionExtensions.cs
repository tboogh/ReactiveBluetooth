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
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Extensions
{
    public static class GattPermissionExtensions
    {
        public static CharacteristicPermission ToCharacteristicPermission(this GattPermission nativePermission)
        {
            CharacteristicPermission permission = 0;
            if (nativePermission.HasFlag(GattPermission.Read))
            {
                permission |= CharacteristicPermission.Read;
            }
            if (nativePermission.HasFlag(GattPermission.ReadEncrypted))
            {
                permission |= CharacteristicPermission.ReadEncrypted;
            }
            if (nativePermission.HasFlag(GattPermission.Write))
            {
                permission |= CharacteristicPermission.Write;
            }
            if (nativePermission.HasFlag(GattPermission.WriteEncrypted))
            {
                permission |= CharacteristicPermission.WriteEncrypted;
            }
            return permission;
        }
    }
}