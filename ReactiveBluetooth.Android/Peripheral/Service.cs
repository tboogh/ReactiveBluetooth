using System;
using Android.Bluetooth;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Service : IService
    {
        public BluetoothGattService GattService { get; }

        public Service(BluetoothGattService gattService)
        {
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