using System;
using System.Security.Cryptography.X509Certificates;

namespace WorkingNameBle.Core
{
    [Flags]
    public enum CharacteristicProperty
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
    public enum CharacteristicPermission
    {
        Read = 1,
        ReadEncrypted,
        Write,
        WriteEncrypted
    }


    public interface ICharacteristic
    {
        Guid Uuid { get; }

        CharacteristicProperty Properties { get; }
        CharacteristicPermission Permissions { get; }
    }
}
