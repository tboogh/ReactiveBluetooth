using System;
using System.Collections.Generic;
using System.Linq;
using Android.Bluetooth;
using Java.Util;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Service : IService
    {
        public Service(Guid id, ServiceType type)
        {
            var gattService = new BluetoothGattService(UUID.FromString(id.ToString()), (GattServiceType)type);
            GattService = gattService;
        }

        public BluetoothGattService GattService { get; }
        public Guid Uuid => Guid.Parse(GattService.Uuid.ToString());
        public ServiceType ServiceType => (ServiceType) GattService.Type;
        public ICharacteristic[] Characteristics { get; private set; }
        public IService[] IncludeServices { get; private set; }

        bool IService.AddCharacteristic(ICharacteristic characteristic)
        {
            List<ICharacteristic> characteristics = Characteristics?.ToList() ?? new List<ICharacteristic>();
            characteristics.Add(characteristic);
            Characteristics = characteristics.ToArray();

            var nativeCharacteristic = (Characteristic) characteristic;
            return GattService.AddCharacteristic(nativeCharacteristic.NativeCharacteristic);
        }

        public bool AddIncludeService(IService service)
        {
            List<IService> services = IncludeServices?.ToList() ?? new List<IService>();
            services.Add(service);
            IncludeServices = services.ToArray();

            return GattService.AddService(((Service) service).GattService);
        }
    }
}