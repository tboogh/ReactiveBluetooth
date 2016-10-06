using System;
using Android.Bluetooth;
using Java.Util;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Service : IService
    {
        public BluetoothGattService GattService { get; }

        
        public Service(Guid id, ServiceType type)
        {
            var gattService = new BluetoothGattService(UUID.FromString(id.ToString()), (GattServiceType)type);
            GattService = gattService;
        }

        public Guid Uuid => Guid.Parse(GattService.Uuid.ToString());
        public ServiceType ServiceType => (ServiceType) GattService.Type;
        
        bool IService.AddCharacteristic(ICharacteristic characteristic)
        {
            var nativeCharacteristic = (Characteristic) characteristic;

            return GattService.AddCharacteristic(nativeCharacteristic.GattCharacteristic);
        }
    }
}