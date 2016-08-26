using System;
using Android.Bluetooth;
using WorkingNameBle.Core;
using IService = WorkingNameBle.Core.Peripheral.IService;

namespace WorkingNameBle.Android.Peripheral
{
    public class Service : IService
    {
        public BluetoothGattService GattService { get; }

        public Service(BluetoothGattService gattService)
        {
            GattService = gattService;
        }

        public Guid Id => Guid.Parse(GattService.Uuid.ToString());
        public ServiceType ServiceType => (ServiceType) GattService.Type;
    }
}