using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS.Extensions
{
    public static class CBAttributePermissionsExtensions
    {
        public static CharacteristicPermission ToCharacteristicPermission(this CBAttributePermissions nativePermissions)
        {
            CharacteristicPermission permission = 0;
            if (nativePermissions.HasFlag(CBAttributePermissions.Readable))
            {
                permission |= CharacteristicPermission.Read;
            }
            if (nativePermissions.HasFlag(CBAttributePermissions.ReadEncryptionRequired))
            {
                permission |= CharacteristicPermission.ReadEncrypted;
            }
            if (nativePermissions.HasFlag(CBAttributePermissions.Writeable))
            {
                permission |= CharacteristicPermission.Write;
            }
            if (nativePermissions.HasFlag(CBAttributePermissions.WriteEncryptionRequired))
            {
                permission |= CharacteristicPermission.WriteEncrypted;
            }

            return permission;
        }
    }
}
