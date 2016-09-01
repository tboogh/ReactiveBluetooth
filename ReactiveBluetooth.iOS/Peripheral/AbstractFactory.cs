using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.iOS.Extensions;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class AbstractFactory : IBluetoothAbstractFactory
    {
        private readonly PeripheralManagerDelegate.PeripheralManagerDelegate _peripheralDelegate;

        public AbstractFactory(PeripheralManagerDelegate.PeripheralManagerDelegate peripheralDelegate)
        {
            _peripheralDelegate = peripheralDelegate;
        }

        public IService CreateService(Guid id, Core.ServiceType type)
        {
            CBMutableService mutableService = new CBMutableService(CBUUID.FromString(id.ToString()), type == ServiceType.Primary);
            return new Service(mutableService);
        }

        public ICharacteristic CreateCharacteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property)
        {
            var nativePermissions = permission.ToCBAttributePermission();
            var nativeProperties = property.ToCBCharacteristicProperty();

            NSData nsData = null;
            if (value != null)
            {
                NSData.FromArray(value);
            }

            CBMutableCharacteristic mutableCharacteristic = new CBMutableCharacteristic(CBUUID.FromString(uuid.ToString()), nativeProperties, nsData, nativePermissions);
            return new Characteristic(mutableCharacteristic, _peripheralDelegate);
        }

        

        
    }
}