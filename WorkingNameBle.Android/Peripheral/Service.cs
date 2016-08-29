using System;
using Android.Bluetooth;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.Android.Peripheral
{
    public class Service : IService
    {
        public BluetoothGattService GattService { get; }

        public Service(BluetoothGattService gattService)
        {
            GattService = gattService;
        }

        public Guid Uuid => Guid.Parse(GattService.Uuid.ToString());
        public Core.ServiceType ServiceType => (Core.ServiceType) GattService.Type;
        
        bool IService.AddCharacteristic(ICharacteristic characteristic)
        {
            var nativeCharacteristic = (Characteristic) characteristic;

            return GattService.AddCharacteristic(nativeCharacteristic.GattCharacteristic);
        }
    }
}