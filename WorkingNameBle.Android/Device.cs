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
using WorkingNameBle.Core;

namespace WorkingNameBle.Android
{
    public class Device : IDevice
    {
        private readonly BluetoothDevice _device;

        public Device(BluetoothDevice device)
        {
            _device = device;
        }
    }
}