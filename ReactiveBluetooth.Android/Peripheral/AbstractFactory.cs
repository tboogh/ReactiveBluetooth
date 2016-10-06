using System;
using System.Reactive.Linq;
using Android.Bluetooth;
using Java.Util;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Android.Peripheral.GattServer;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class AbstractFactory : IBluetoothAbstractFactory
    {
        private readonly IServerCallback _serverCallback;

        public AbstractFactory(IServerCallback serverCallback)
        {
            _serverCallback = serverCallback;
        }

        public IService CreateService(Guid uuid, ServiceType type)
        {
            return new Service(uuid, type);
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
            Characteristic characteristic = new Characteristic(uuid, value, permission, property, _serverCallback);
            return characteristic;
        }

        public IDescriptor CreateDescriptor(Guid uuid, byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}