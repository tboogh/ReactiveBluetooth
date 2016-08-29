using System;

namespace WorkingNameBle.Core
{
    [Flags]
    public enum CharacteristicProperties
    {
        Broadcast = 1,
        Read,
        WriteWithoutResponse,
        Write,
        Notify,
        Indicate,
        AuthenticatedSignedWrites,
        ExtendedProperties
    }
    [Flags]
    public enum CharacteristicPermissions
    {
        Read = 1,
        ReadEncrypted,
        Write,
        WriteEncrypted
    }


    public interface ICharacteristic
    {
        Guid Uuid { get; }
    }
}
