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

        public CBMutableService MutableService { get; }
        public Guid Uuid => Guid.Parse(MutableService.UUID.ToString());
        public ServiceType ServiceType => MutableService.Primary ? ServiceType.Primary : ServiceType.Secondary;

        public bool AddCharacteristic(ICharacteristic characteristic)
        {
            List<CBCharacteristic> characteristics = MutableService.Characteristics?.ToList() ?? new List<CBCharacteristic>();
            var nativeCharacteristic = (Characteristic) characteristic;
            characteristics.Add(nativeCharacteristic.MutableCharacteristic);

            MutableService.Characteristics = characteristics.ToArray();
            return true;
        }
    }
}