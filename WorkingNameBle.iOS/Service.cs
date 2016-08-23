using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS
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

        public Guid Id => Guid.Parse(_service.UUID.ToString());

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