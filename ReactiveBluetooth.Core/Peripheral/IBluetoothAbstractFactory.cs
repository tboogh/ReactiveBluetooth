using System;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.Core.Peripheral
{
    public interface IBluetoothAbstractFactory
    {
        IService CreateService(Guid id, ServiceType type);
        ICharacteristic CreateCharacteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property);
    }
}
