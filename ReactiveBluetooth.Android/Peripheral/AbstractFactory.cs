using System;
using System.Reactive.Linq;
using Android.Bluetooth;
using Java.Util;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Android.Peripheral.GattServer;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class AbstractFactory : IBluetoothAbstractFactory
    {
        private readonly ServerCallback _serverCallback;

        public AbstractFactory(ServerCallback serverCallback)
        {
            _serverCallback = serverCallback;
        }

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
        /// <param name="permission">Permissions</param>
        /// <param name="property">Properties</param>
        /// <exception cref="Exception">Throws excpetion if characteristic value cannot be set</exception>
        /// <returns></returns>
        public ICharacteristic CreateCharacteristic(Guid uuid, byte[] value, CharacteristicPermission permission, CharacteristicProperty property)
        {
            var nativePermissions = permission.ToGattPermission();
            var nativeProperties = property.ToGattProperty();

            var bluetoothGattCharacteristic = new BluetoothGattCharacteristic(UUID.FromString(uuid.ToString()), nativeProperties, nativePermissions);

            Characteristic characteristic = new Characteristic(bluetoothGattCharacteristic, _serverCallback);

            if (!bluetoothGattCharacteristic.SetValue(value))
            {
                throw new Exception("Failed to set characteristic value");
            }
            return characteristic;
        }

        

        
    }
}