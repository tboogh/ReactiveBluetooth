using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Peripheral;
using ICharacteristic = WorkingNameBle.Core.Peripheral.ICharacteristic;
using IService = WorkingNameBle.Core.Peripheral.IService;

namespace WorkingNameBle.iOS.Peripheral
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
            var nativePermissions = ConvertPermissions(permission);
            var nativeProperties = ConvertProperties(property);

            var nsData = NSData.FromArray(value);

            CBMutableCharacteristic mutableCharacteristic = new CBMutableCharacteristic(CBUUID.FromString(uuid.ToString()), nativeProperties, nsData, nativePermissions);
            return new Characteristic(mutableCharacteristic);
        }

        public CBAttributePermissions ConvertPermissions(CharacteristicPermission permission)
        {
            CBAttributePermissions nativePermissions= 0;
            if (permission.HasFlag(CharacteristicPermission.Read))
            {
                nativePermissions |= CBAttributePermissions.Readable;
            }
            if (permission.HasFlag(CharacteristicPermission.ReadEncrypted))
            {
                nativePermissions |= CBAttributePermissions.ReadEncryptionRequired;
            }
            if (permission.HasFlag(CharacteristicPermission.Write))
            {
                nativePermissions |= CBAttributePermissions.Writeable;
            }
            if (permission.HasFlag(CharacteristicPermission.WriteEncrypted))
            {
                nativePermissions |= CBAttributePermissions.WriteEncryptionRequired;
            }

            return nativePermissions;
        }

        public CBCharacteristicProperties ConvertProperties(CharacteristicProperty property)
        {
            CBCharacteristicProperties nativeProperties = 0;

            if (property.HasFlag(CharacteristicProperty.Broadcast))
            {
                nativeProperties |= CBCharacteristicProperties.Broadcast;
            }
            if (property.HasFlag(CharacteristicProperty.Read))
            {
                nativeProperties |= CBCharacteristicProperties.Read;
            }
            if (property.HasFlag(CharacteristicProperty.WriteWithoutResponse))
            {
                nativeProperties |= CBCharacteristicProperties.WriteWithoutResponse;
            }
            if (property.HasFlag(CharacteristicProperty.Write))
            {
                nativeProperties |= CBCharacteristicProperties.Write;
            }
            if (property.HasFlag(CharacteristicProperty.Notify))
            {
                nativeProperties |= CBCharacteristicProperties.Notify;
            }
            if (property.HasFlag(CharacteristicProperty.Indicate))
            {
                nativeProperties |= CBCharacteristicProperties.Indicate;
            }
            if (property.HasFlag(CharacteristicProperty.AuthenticatedSignedWrites))
            {
                nativeProperties |= CBCharacteristicProperties.AuthenticatedSignedWrites;
            }
            if (property.HasFlag(CharacteristicProperty.ExtendedProperties))
            {
                nativeProperties |= CBCharacteristicProperties.ExtendedProperties;
            }

            return nativeProperties;
        }
    }
}