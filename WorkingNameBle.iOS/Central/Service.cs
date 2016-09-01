using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CoreBluetooth;
using WorkingNameBle.Core;
using IService = WorkingNameBle.Core.Central.IService;

namespace WorkingNameBle.iOS.Central
{
    public class Service : IService
    {
        private readonly CBService _service;
        private readonly CBPeripheral _nativeDevice;

        public Service(CBService service, CBPeripheral nativeDevice)
        {
            _service = service;
            _nativeDevice = nativeDevice;
        }

        public Guid Uuid => Guid.Parse(_service.UUID.ToString());
        public ServiceType ServiceType => _service.Primary ? ServiceType.Primary : ServiceType.Secondary;

        public IObservable<IList<ICharacteristic>> DiscoverCharacteristics()
        {
            var observable = Observable.FromEventPattern<EventHandler<CBServiceEventArgs>, CBServiceEventArgs>(eh => _nativeDevice.DiscoveredCharacteristic += eh, eh => _nativeDevice.DiscoveredCharacteristic -= eh)
                .Select(x => _service.Characteristics.Select(characteristic => new Characteristic(characteristic))
                    .Cast<ICharacteristic>()
                    .ToList());

            _nativeDevice.DiscoverCharacteristics(_service);

            return observable;
        }
    }
}