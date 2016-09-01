using Android.Bluetooth;

namespace WorkingNameBle.Android.Peripheral.GattServer
{
    public class DescriptorWriteRequest
    {
        public DescriptorWriteRequest(BluetoothDevice bluetoothDevice, int requestId, BluetoothGattDescriptor bluetoothGattDescriptor, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            PreparedWrite = preparedWrite;
            ResponseNeeded = responseNeeded;
            Offset = offset;
            Value = value;
            BluetoothDevice = bluetoothDevice;
            RequestId = requestId;
            BluetoothGattDescriptor = bluetoothGattDescriptor;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public int RequestId { get; }
        public BluetoothGattDescriptor BluetoothGattDescriptor { get; }
        public bool ResponseNeeded { get; }
        public int Offset { get; }
        public byte[] Value { get; }
        public bool PreparedWrite { get; }
    }
}