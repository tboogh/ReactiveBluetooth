using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS
{
    public class Device : IDevice
    {
        public Device(CBPeripheral nativeDevice)
        {
            NativeDevice = nativeDevice;
            Name = nativeDevice.Name;
        }

        public CBPeripheral NativeDevice { get; }

        public string Name { get; }
    }
}
