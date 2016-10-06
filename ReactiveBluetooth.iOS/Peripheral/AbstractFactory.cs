using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
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

        public IService CreateService(Guid uuid, ServiceType type)
        {
            
            return new Service(uuid, type);
        }

        public ICharacteristic CreateCharacteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property)
        {
            return new Characteristic(uuid, value, permission, property, _peripheralDelegate);
        }

        public IDescriptor CreateDescriptor(Guid uuid, byte[] value)
        {
            return new Descriptor(uuid, value);
        }
    }
}