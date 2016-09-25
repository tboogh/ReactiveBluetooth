using System;

namespace ReactiveBluetooth.Core.Types
{
    [Flags]
    public enum CharacteristicPermission
    {
        Read = 1,
        ReadEncrypted = 2,
        Write = 4,
        WriteEncrypted = 8
    }
}