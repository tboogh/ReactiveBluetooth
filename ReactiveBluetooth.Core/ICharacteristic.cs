using System;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core
{
    public interface ICharacteristic
    {
        Guid Uuid { get; }

        CharacteristicProperty Properties { get; }

        IDescriptor[] Descriptors { get; }
    }
}
