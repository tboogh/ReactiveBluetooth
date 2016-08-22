using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS
{
    public class Device : IDevice
    {
        private readonly CBPeripheral _peripheral;

        public Device(CBPeripheral peripheral)
        {
            _peripheral = peripheral;
        }
    }
}
