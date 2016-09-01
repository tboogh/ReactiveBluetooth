using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Android.Bluetooth;
using WorkingNameBle.Core;
using IService = WorkingNameBle.Core.Central.IService;

namespace WorkingNameBle.Android.Central
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

        public IObservable<IList<ICharacteristic>> DiscoverCharacteristics()
        {
            return Observable.Return(_service.Characteristics.Select(characteristic => new Characteristic(characteristic)))
                .Cast<ICharacteristic>().ToList();
        }
    }
}