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
using Java.Util;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Descriptor : IDescriptor
    {
        public Descriptor(BluetoothGattDescriptor descriptor)
        {
            NativeDescriptor = descriptor;
        }

        public Descriptor(Guid uuid, byte[] value, DescriptorPermission permission)
        {
            BluetoothGattDescriptor gattDescriptor = new BluetoothGattDescriptor(UUID.FromString(uuid.ToString()), permission.ToGattPermission());
            gattDescriptor.SetValue(value);
            NativeDescriptor = gattDescriptor;
        }

        public Guid Uuid => NativeDescriptor.Uuid.ToString()
            .ToGuid();

        public BluetoothGattDescriptor NativeDescriptor { get; }
    }
}