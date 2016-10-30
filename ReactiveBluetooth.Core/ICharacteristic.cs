using System;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core
{
    /// <summary>
    /// Base interface for characteristic
    /// </summary>
    public interface ICharacteristic
    {
        /// <summary>
        /// UUID of characteristic
        /// </summary>
        Guid Uuid { get; }

        /// <summary>
        /// The <see cref="CharacteristicProperty"/> supported by the characteristic
        /// </summary>
        CharacteristicProperty Properties { get; }

        /// <summary>
        /// Array of <see cref="IDescriptor"/>s of the characteristic
        /// </summary>
        IDescriptor[] Descriptors { get; }
    }
}
