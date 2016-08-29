using System;
using Android.Bluetooth;
using Java.Util;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Peripheral;
using ICharacteristic = WorkingNameBle.Core.Peripheral.ICharacteristic;
using IService = WorkingNameBle.Core.Peripheral.IService;

namespace WorkingNameBle.Android.Peripheral
{
    public class AbstractFactory : IBluetoothAbstractFactory
    {
        public IService CreateService(Guid id, Core.ServiceType type)
        {
            var gattService = new BluetoothGattService(UUID.FromString(id.ToString()), (GattServiceType)type);
            return new Service(gattService);
        }

        /// <summary>
        /// Creates a new characteristic with a native backing
        /// </summary>
        /// <param name="uuid">Uuid</param>
        /// <param name="value">data</param>
        /// <param name="permissions">Permissions</param>
        /// <param name="properties">Properties</param>
        /// <exception cref="Exception">Throws excpetion if characteristic value cannot be set</exception>
        /// <returns></returns>
        public ICharacteristic CreateCharacteristic(Guid uuid, byte[] value, CharacteristicPermissions permissions, CharacteristicProperties properties)
        {
            var nativePermissions = ConvertPermissions(permissions);
            var nativeProperties = ConvertProperties(properties);
            var characteristic = new BluetoothGattCharacteristic(UUID.FromString(uuid.ToString()), nativeProperties, nativePermissions);
            if (!characteristic.SetValue(value))
            {
                throw new Exception("Failed to set characteristic value");
            }
            return new Characteristic(characteristic);
        }

        public GattProperty ConvertProperties(CharacteristicProperties properties)
        {
            GattProperty nativeProperties = 0;
            if (properties.HasFlag(CharacteristicProperties.Broadcast))
            {
                nativeProperties |= GattProperty.Broadcast;
            }
            if (properties.HasFlag(CharacteristicProperties.Read))
            {
                nativeProperties |= GattProperty.Read;
            }
            if (properties.HasFlag(CharacteristicProperties.WriteWithoutResponse))
            {
                nativeProperties |= GattProperty.WriteNoResponse;
            }
            if (properties.HasFlag(CharacteristicProperties.Write))
            {
                nativeProperties |= GattProperty.Write;
            }
            if (properties.HasFlag(CharacteristicProperties.Notify))
            {
                nativeProperties |= GattProperty.Notify;
            }
            if (properties.HasFlag(CharacteristicProperties.Indicate))
            {
                nativeProperties |= GattProperty.Indicate;
            }
            if (properties.HasFlag(CharacteristicProperties.AuthenticatedSignedWrites))
            {
                nativeProperties |= GattProperty.SignedWrite;
            }
            if (properties.HasFlag(CharacteristicProperties.ExtendedProperties))
            {
                nativeProperties |= GattProperty.ExtendedProps;
            }
            return nativeProperties;
        }

        public GattPermission ConvertPermissions(CharacteristicPermissions permissions)
        {
            GattPermission nativePermissions = 0;
            if (permissions.HasFlag(CharacteristicPermissions.Read))
            {
                nativePermissions |= GattPermission.Read;
            }
            if (permissions.HasFlag(CharacteristicPermissions.ReadEncrypted))
            {
                nativePermissions |= GattPermission.ReadEncrypted;
            }
            if (permissions.HasFlag(CharacteristicPermissions.Write))
            {
                nativePermissions |= GattPermission.Write;
            }
            if (permissions.HasFlag(CharacteristicPermissions.WriteEncrypted))
            {
                nativePermissions |= GattPermission.WriteEncrypted;
            }
            return nativePermissions;
        }
    }
}