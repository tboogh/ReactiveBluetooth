using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public class DescriptorReadRequest
    {
        public DescriptorReadRequest(BluetoothDevice bluetoothDevice, int requestId, BluetoothGattDescriptor bluetoothGattDescriptor)
        {
            BluetoothDevice = bluetoothDevice;
            RequestId = requestId;
            BluetoothGattDescriptor = bluetoothGattDescriptor;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public int RequestId { get; }
        public BluetoothGattDescriptor BluetoothGattDescriptor { get; }
    }
}