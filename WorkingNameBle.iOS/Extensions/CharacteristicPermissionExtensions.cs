using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS.Extensions
{
    public static class CharacteristicPermissionExtensions
    {
        public static CBAttributePermissions ToCBAttributePermission(this CharacteristicPermission permission)
        {
            CBAttributePermissions nativePermissions = 0;
            if (permission.HasFlag(CharacteristicPermission.Read))
            {
                nativePermissions |= CBAttributePermissions.Readable;
            }
            if (permission.HasFlag(CharacteristicPermission.ReadEncrypted))
            {
                nativePermissions |= CBAttributePermissions.ReadEncryptionRequired;
            }
            if (permission.HasFlag(CharacteristicPermission.Write))
            {
                nativePermissions |= CBAttributePermissions.Writeable;
            }
            if (permission.HasFlag(CharacteristicPermission.WriteEncrypted))
            {
                nativePermissions |= CBAttributePermissions.WriteEncryptionRequired;
            }

            return nativePermissions;
        }
    }
}