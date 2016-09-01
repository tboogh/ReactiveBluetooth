using System;
using System.Security.Cryptography.X509Certificates;

namespace ReactiveBluetooth.Core
{
    [Flags]
    public enum CharacteristicProperty
    {
        Broadcast = 1,
        Read = 2,
        WriteWithoutResponse = 4,
        Write = 8,
        Notify = 16,
        Indicate = 32,
        AuthenticatedSignedWrites = 64,
        ExtendedProperties = 128
    }

    [Flags]
    public enum CharacteristicPermission
    {
        Read = 1,
        ReadEncrypted = 2,
        Write = 4,
        WriteEncrypted = 8
    }


    public interface ICharacteristic
    {
        Guid Uuid { get; }

        CharacteristicProperty Properties { get; }
        CharacteristicPermission Permissions { get; }
    }
}
