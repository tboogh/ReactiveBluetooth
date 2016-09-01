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
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.Android.Peripheral
{
    public class AttRequest : IAttRequest
    {
        public AttRequest(ICharacteristic characteristic, int offset, byte[] value, int requestId, BluetoothDevice bluetoothDevice)
        {
            Characteristic = characteristic;
            Offset = offset;
            Value = value;
            RequestId = requestId;
            BluetoothDevice = bluetoothDevice;
        }

        public ICharacteristic Characteristic { get; }
        public int Offset { get; }
        public byte[] Value { get; }
        public int RequestId { get; }
        public BluetoothDevice BluetoothDevice { get; }
    }
}