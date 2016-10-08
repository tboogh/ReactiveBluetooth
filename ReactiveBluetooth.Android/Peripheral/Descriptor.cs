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
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Descriptor : IDescriptor
    {
        public Descriptor(BluetoothGattDescriptor descriptor)
        {
            NativeDescriptor = descriptor;
        }

        public Descriptor(Guid uuid)
        {
            BluetoothGattDescriptor gattDescriptor = new BluetoothGattDescriptor(UUID.FromString(uuid.ToString()), GattDescriptorPermission.Read);
            NativeDescriptor = gattDescriptor;
            throw new NotImplementedException("Implementation not completed");
        }

        public Guid Uuid => NativeDescriptor.Uuid.ToString()
            .ToGuid();

        public BluetoothGattDescriptor NativeDescriptor { get; }
    }
}