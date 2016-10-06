using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class Descriptor : IDescriptor
    {
        public Descriptor(Guid uuid, byte[] value)
        {
            MutableDescriptor = new CBMutableDescriptor(CBUUID.FromString(uuid.ToString()), NSData.FromArray(value));
        }

        public Guid Uuid => MutableDescriptor.UUID.Uuid.ToGuid();

        public CBMutableDescriptor MutableDescriptor { get; }
    }
}
