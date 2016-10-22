using System;

namespace ReactiveBluetooth.Core.Types
{
    [Flags]
    public enum DescriptorPermission
    {
        Read = 1,
        ReadEncrypted = 2,
        Write = 4,
        WriteEncrypted = 8
    }
}