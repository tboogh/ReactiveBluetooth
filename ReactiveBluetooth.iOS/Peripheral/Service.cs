using System;
using System.Collections.Generic;
using System.Linq;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Types;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class Service : IService
    {
        public Service(CBMutableService mutableService)
        {
            MutableService = mutableService;
        }

        public Service(Guid id, ServiceType type)
        {
            CBMutableService mutableService = new CBMutableService(CBUUID.FromString(id.ToString()), type == ServiceType.Primary);
            MutableService = mutableService;
        }

        public CBMutableService MutableService { get; }
        public Guid Uuid => Guid.Parse(MutableService.UUID.ToString());
        public ServiceType ServiceType => MutableService.Primary ? ServiceType.Primary : ServiceType.Secondary;

        public bool AddCharacteristic(ICharacteristic characteristic)
        {
            List<ICharacteristic> characteristics = Characteristics?.ToList() ?? new List<ICharacteristic>();
            characteristics.Add(characteristic);
            Characteristics = characteristics.ToArray();

            List<CBCharacteristic> nativeCharacteristics = MutableService.Characteristics?.ToList() ?? new List<CBCharacteristic>();
            var nativeCharacteristic = (Characteristic) characteristic;
            nativeCharacteristics.Add(nativeCharacteristic.NativeCharacteristic);

            MutableService.Characteristics = nativeCharacteristics.ToArray();
            return true;
        }

        public bool AddIncludeService(IService service)
        {
            List<IService> services = IncludeServices?.ToList() ?? new List<IService>();
            services.Add(service);
            IncludeServices = services.ToArray();

            List<CBService> nativeServices = MutableService.IncludedServices?.ToList() ?? new List<CBService>();
            nativeServices.Add(((Service)service).MutableService);
            MutableService.IncludedServices = nativeServices.ToArray();

            return true;
        }

        public ICharacteristic[] Characteristics { get; private set; }
        public IService[] IncludeServices { get; private set; }
    }
}