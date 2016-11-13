using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Types;
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

        public Guid Uuid
        {
            get { return _service.UUID.Uuid.ToGuid(); }
        }

        public ServiceType ServiceType => _service.Primary ? ServiceType.Primary : ServiceType.Secondary;

        public Task<IList<ICharacteristic>> DiscoverCharacteristics(CancellationToken cancellationToken)
        {
            var observable = Observable.FromEvent<IList<ICharacteristic>>(action => { _nativeDevice.DiscoverCharacteristics(_service); }, _ => { });

            IObservable<IList<ICharacteristic>> delegateObservable = _cbPeripheralDelegate.DiscoveredCharacteristicsSubject
                .Where(x => Equals(x.Item2, _service) && Equals(x.Item1, _nativeDevice))
                .Select(x => x.Item2.Characteristics.Select(y => new Characteristic(y))
                    .Cast<ICharacteristic>()
                    .ToList());

            return observable.Merge(delegateObservable)
                .Take(1)
                .ToTask(cancellationToken);
        }

        public Task<IList<IService>> DiscoverIncludedServices(CancellationToken cancellationToken, IList<Guid> serviceUuids = null)
        {
            var observable = Observable.FromEvent<IList<IService>>(action =>
            {
                _nativeDevice.DiscoverIncludedServices(serviceUuids?.Select(x => CBUUID.FromString(x.ToString()))
                    .ToArray(), _service);
            }, _ => { });

            IObservable<IList<IService>> delegateObservable = _cbPeripheralDelegate.DiscoveredIncludedServicesSubject
                .Where(x => Equals(x.Item2, _service) && Equals(x.Item1, _nativeDevice))
                .Select(x => x.Item2.IncludedServices.Select(y => new Service(y, _nativeDevice, _cbPeripheralDelegate))
                    .Cast<IService>()
                    .ToList());

            return observable.Merge(delegateObservable)
                .Take(1)
                .ToTask(cancellationToken);
        }
    }
}