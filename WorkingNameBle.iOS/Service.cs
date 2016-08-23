using System;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS
{
    public class Service : IService
    {
        private readonly CBService _service;

        public Service(CBService service)
        {
            _service = service;
        }

        public Guid Id => Guid.Parse(_service.UUID.ToString());
    }
}