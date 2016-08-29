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
        public IService CreateService(Guid id, Core.ServiceType type)
        {
            CBMutableService mutableService = new CBMutableService(CBUUID.FromString(id.ToString()), type == ServiceType.Primary);
            return new Service(mutableService);
        }

        public ICharacteristic CreateCharacteristic(Guid uuid, byte[] value, CharacteristicPermissions permissions, CharacteristicProperties properties)
        {
            var nativePermissions = ConvertPermissions(permissions);
            var nativeProperties = ConvertProperties(properties);

            var nsData = NSData.FromArray(value);

            CBMutableCharacteristic mutableCharacteristic = new CBMutableCharacteristic(CBUUID.FromString(uuid.ToString()), nativeProperties, nsData, nativePermissions);
            return new Characteristic(mutableCharacteristic);
        }

        public CBAttributePermissions ConvertPermissions(CharacteristicPermissions permissions)
        {
            CBAttributePermissions nativePermissions= 0;
            if (permissions.HasFlag(CharacteristicPermissions.Read))
            {
                nativePermissions |= CBAttributePermissions.Readable;
            }
            if (permissions.HasFlag(CharacteristicPermissions.ReadEncrypted))
            {
                nativePermissions |= CBAttributePermissions.ReadEncryptionRequired;
            }
            if (permissions.HasFlag(CharacteristicPermissions.Write))
            {
                nativePermissions |= CBAttributePermissions.Writeable;
            }
            if (permissions.HasFlag(CharacteristicPermissions.WriteEncrypted))
            {
                nativePermissions |= CBAttributePermissions.WriteEncryptionRequired;
            }

            return nativePermissions;
        }

        public CBCharacteristicProperties ConvertProperties(CharacteristicProperties properties)
        {
            CBCharacteristicProperties nativeProperties = 0;

            if (properties.HasFlag(CharacteristicProperties.Broadcast))
            {
                nativeProperties |= CBCharacteristicProperties.Broadcast;
            }
            if (properties.HasFlag(CharacteristicProperties.Read))
            {
                nativeProperties |= CBCharacteristicProperties.Read;
            }
            if (properties.HasFlag(CharacteristicProperties.WriteWithoutResponse))
            {
                nativeProperties |= CBCharacteristicProperties.WriteWithoutResponse;
            }
            if (properties.HasFlag(CharacteristicProperties.Write))
            {
                nativeProperties |= CBCharacteristicProperties.Write;
            }
            if (properties.HasFlag(CharacteristicProperties.Notify))
            {
                nativeProperties |= CBCharacteristicProperties.Notify;
            }
            if (properties.HasFlag(CharacteristicProperties.Indicate))
            {
                nativeProperties |= CBCharacteristicProperties.Indicate;
            }
            if (properties.HasFlag(CharacteristicProperties.AuthenticatedSignedWrites))
            {
                nativeProperties |= CBCharacteristicProperties.AuthenticatedSignedWrites;
            }
            if (properties.HasFlag(CharacteristicProperties.ExtendedProperties))
            {
                nativeProperties |= CBCharacteristicProperties.ExtendedProperties;
            }

            return nativeProperties;
        }
    }
}