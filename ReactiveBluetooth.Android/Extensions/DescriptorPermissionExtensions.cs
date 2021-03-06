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
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Extensions
{
    public static class DescriptorPermissionExtensions
    {
        public static GattDescriptorPermission ToGattPermission(this DescriptorPermission permission)
        {
            GattDescriptorPermission nativePermissions = 0;
            if (permission.HasFlag(DescriptorPermission.Read))
            {
                nativePermissions |= GattDescriptorPermission.Read;
            }
            if (permission.HasFlag(DescriptorPermission.ReadEncrypted))
            {
                nativePermissions |= GattDescriptorPermission.ReadEncrypted;
            }
            if (permission.HasFlag(DescriptorPermission.Write))
            {
                nativePermissions |= GattDescriptorPermission.Write;
            }
            if (permission.HasFlag(DescriptorPermission.WriteEncrypted))
            {
                nativePermissions |= GattDescriptorPermission.WriteEncrypted;
            }
            return nativePermissions;
        }
    }
}