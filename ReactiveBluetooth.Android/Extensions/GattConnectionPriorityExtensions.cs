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
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Android.Extensions
{
    public static class GattConnectionPriorityExtensions
    {
        public static GattConnectionPriority ToConnectionPriority(this ConnectionPriority priority)
        {
            switch (priority)
            {
                case ConnectionPriority.Balanced:
                    return GattConnectionPriority.Balanced;
                case ConnectionPriority.Low:
                    return GattConnectionPriority.LowPower;
                case ConnectionPriority.HighPower:
                    return GattConnectionPriority.High;
                default:
                    throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
            }
        }
    }
}