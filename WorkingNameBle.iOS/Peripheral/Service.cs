using System;
using CoreBluetooth;
using WorkingNameBle.Core;
using IService = WorkingNameBle.Core.Peripheral.IService;

namespace WorkingNameBle.iOS.Peripheral
{
    public class Service : IService
    {
        public CBMutableService MutableService { get; }

        public Service(CBMutableService mutableService)
        {
            MutableService = mutableService;
        }

        public Guid Id => Guid.Parse(MutableService.UUID.ToString());
        public ServiceType ServiceType => MutableService.Primary ? ServiceType.Primary : ServiceType.Secondary;
    }
}