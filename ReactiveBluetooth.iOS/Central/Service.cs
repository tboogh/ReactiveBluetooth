using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace ReactiveBluetooth.iOS.Central
{
    public class Service : IService
    {
        private readonly CBService _service;
        private readonly CBPeripheral _nativeDevice;
        private readonly PeripheralDelegate.PeripheralDelegate _cbPeripheralDelegate;

        public Service(CBService service, CBPeripheral nativeDevice, PeripheralDelegate.PeripheralDelegate cbPeripheralDelegate)
        {
            _service = service;
            _nativeDevice = nativeDevice;
            _cbPeripheralDelegate = cbPeripheralDelegate;
        }

        public Guid Uuid => Guid.Parse(_service.UUID.Uuid);
        public ServiceType ServiceType => _service.Primary ? ServiceType.Primary : ServiceType.Secondary;

        public IObservable<IList<ICharacteristic>> DiscoverCharacteristics()
        {
            var observable = Observable.FromEvent<IList<ICharacteristic>>(action =>
            { _nativeDevice.DiscoverCharacteristics(_service); }, _ => { });

            IObservable<IList<ICharacteristic>> delegateObservable =
                _cbPeripheralDelegate.DiscoveredCharacteristicsSubject.Select(
                    x => x.Item2.Characteristics.Select(y => new Characteristic(y)).Cast<ICharacteristic>().ToList());

            return observable.Merge(delegateObservable).FirstAsync();
        }
    }
}