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

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Descriptor : IDescriptor
    {
        public Descriptor(Guid uuid)
        {
            BluetoothGattDescriptor gattDescriptor = new BluetoothGattDescriptor(UUID.FromString(uuid.ToString()), GattDescriptorPermission.Read);
        }

        public Guid Uuid { get; }
    }
}