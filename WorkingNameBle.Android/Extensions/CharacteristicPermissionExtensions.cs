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
using WorkingNameBle.Core;

namespace WorkingNameBle.Android.Extensions
{
    public static class CharacteristicPermissionExtensions
    {
        public static GattPermission ToGattPermission(this CharacteristicPermission permission)
        {
            GattPermission nativePermissions = 0;
            if (permission.HasFlag(CharacteristicPermission.Read))
            {
                nativePermissions |= GattPermission.Read;
            }
            if (permission.HasFlag(CharacteristicPermission.ReadEncrypted))
            {
                nativePermissions |= GattPermission.ReadEncrypted;
            }
            if (permission.HasFlag(CharacteristicPermission.Write))
            {
                nativePermissions |= GattPermission.Write;
            }
            if (permission.HasFlag(CharacteristicPermission.WriteEncrypted))
            {
                nativePermissions |= GattPermission.WriteEncrypted;
            }
            return nativePermissions;
        }
    }
}