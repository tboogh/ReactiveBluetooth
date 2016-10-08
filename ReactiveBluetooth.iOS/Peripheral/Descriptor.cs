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
            NativeDescriptor = new CBMutableDescriptor(CBUUID.FromString(uuid.ToString()), NSData.FromArray(value));
        }

        public Descriptor(CBDescriptor descriptor)
        {
            NativeDescriptor = descriptor;
        }

        public Guid Uuid => NativeDescriptor.UUID.Uuid.ToGuid();

        /// <summary>
        /// CBMutableDescriptor has no writeable properties (last checked in iOS 10)
        /// </summary>
        public CBDescriptor NativeDescriptor { get; }
    }
}
