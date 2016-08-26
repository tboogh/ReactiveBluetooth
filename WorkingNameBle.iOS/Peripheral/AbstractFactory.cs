using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Peripheral;
using ICharacteristic = WorkingNameBle.Core.Peripheral.ICharacteristic;
using IService = WorkingNameBle.Core.Peripheral.IService;

namespace WorkingNameBle.iOS.Peripheral
{
    public class AbstractFactory : IBluetoothAbstractFactory
    {
        public IService CreateService(Guid id, Core.ServiceType type)
        {
            CBMutableService mutableService = new CBMutableService(CBUUID.FromString(id.ToString()), type == ServiceType.Primary);
            return new Service(mutableService);
        }

        public ICharacteristic CreateCharacteristic()
        {
            return new Characteristic();
        }
    }
}