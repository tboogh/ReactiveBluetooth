using System;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Core.Peripheral
{
    public interface IBluetoothAbstractFactory
    {
        IService CreateService(Guid id, ServiceType type);
        ICharacteristic CreateCharacteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property);
    }
}
