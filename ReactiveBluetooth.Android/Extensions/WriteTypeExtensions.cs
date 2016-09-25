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
    public static class WriteTypeExtensions
    {
        public static GattWriteType ToGattWriteType(this WriteType writeType)
        {
            switch (writeType)
            {
                case WriteType.WithoutRespoonse:
                    return GattWriteType.Default;
                case WriteType.WithResponse:
                    return GattWriteType.NoResponse;
                default:
                    throw new ArgumentOutOfRangeException(nameof(writeType), writeType, null);
            }
        }
    }
}