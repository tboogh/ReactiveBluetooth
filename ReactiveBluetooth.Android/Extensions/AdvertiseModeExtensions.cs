using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ReactiveBluetooth.Android.Extensions
{
    public static class AdvertiseModeExtensions
    {
        public static AdvertiseMode ToAdvertiseMode(this Core.Types.AdvertiseMode advertiseMode)
        {
            switch (advertiseMode)
            {
                case Core.Types.AdvertiseMode.Balanced:
                    return AdvertiseMode.Balanced;
                case Core.Types.AdvertiseMode.LowPower:
                    return AdvertiseMode.LowPower;
                case Core.Types.AdvertiseMode.LowLatency:
                    return AdvertiseMode.LowLatency;
                default:
                    throw new ArgumentOutOfRangeException(nameof(advertiseMode), advertiseMode, null);
            }
        }
    }
}