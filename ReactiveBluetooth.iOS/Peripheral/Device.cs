using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Peripheral;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class Device : IDevice
    {
        public CBCentral NativeCentral { get; }

        public Device(CBCentral nativeCentral)
        {
            this.NativeCentral = nativeCentral;
        }

        public Guid Uuid => NativeCentral.Identifier.ToString()
            .ToGuid();
    }
}
