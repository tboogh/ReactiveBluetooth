using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Types;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace ReactiveBluetooth.Android.Central
{
    public class Service : IService
    {
        private readonly BluetoothGattService _service;

        public Service(BluetoothGattService service)
        {
            _service = service;
        }

        public Guid Uuid
        {
            get
            {
                var uuid = _service.Uuid.ToString();
                return Guid.Parse(uuid);
            }
        }

        public ServiceType ServiceType => (ServiceType) _service.Type;

        public Task<IList<ICharacteristic>> DiscoverCharacteristics(CancellationToken cancellationToken)
        {
            var obs =
                Observable.Return<IList<ICharacteristic>>(
                    _service.Characteristics.Select(characteristic => new Characteristic(characteristic))
                        .Cast<ICharacteristic>()
                        .ToList());
            return obs.ToTask(cancellationToken);
        }
    }
}