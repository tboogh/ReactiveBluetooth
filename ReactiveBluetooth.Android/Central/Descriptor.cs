using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;

namespace ReactiveBluetooth.Android.Central
{
    public class Descriptor : IDescriptor
    {
        public BluetoothGattDescriptor NativeDescriptor { get; }

        public Descriptor(BluetoothGattDescriptor gattDescriptor)
        {
            NativeDescriptor = gattDescriptor;
        }

        public Guid Uuid => NativeDescriptor.Uuid.ToString().ToGuid();
    }
}