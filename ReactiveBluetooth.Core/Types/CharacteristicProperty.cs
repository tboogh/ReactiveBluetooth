using System;

namespace ReactiveBluetooth.Core.Types
{
    /// <summary>
    /// Characteristic properties
    /// </summary>
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
}