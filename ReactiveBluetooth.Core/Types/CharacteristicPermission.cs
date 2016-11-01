using System;

namespace ReactiveBluetooth.Core.Types
{

    /// <summary>
    /// Characteristic permissions
    /// </summary>
    [Flags]
    public enum CharacteristicPermission
    {
        /// <summary>
        /// The characterstic supports reading
        /// </summary>
        Read = 1,
        /// <summary>
        /// The characterstic supports encrypted reading
        /// </summary>
        ReadEncrypted = 2,
        /// <summary>
        /// The characterstic supports writing
        /// </summary>
        Write = 4,
        /// <summary>
        /// The characterstic supports encrypted reading
        /// </summary>
        WriteEncrypted = 8
    }
}