using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;

namespace ReactiveBluetooth.iOS.Central
{
    public class Descriptor : IDescriptor
    {
        public CBDescriptor NativeDescriptor { get; }

        public Descriptor(CBDescriptor descriptor)
        {
            NativeDescriptor = descriptor;
        }

        public Guid Uuid => NativeDescriptor.UUID.Uuid.ToGuid();
    }
}