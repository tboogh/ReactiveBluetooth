using System;
using Android.Bluetooth;
using Java.Util;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.Android.Peripheral
{
    public class AbstractFactory : IBluetoothAbstractFactory
    {
        public IService CreateService(Guid id, Core.ServiceType type)
        {
            var gattService = new BluetoothGattService(UUID.FromString(id.ToString()), (GattServiceType)type);
            return new Service(gattService);
        }

        public ICharacteristic CreateCharacteristic()
        {
            return new Characteristic();
        }
    }
}