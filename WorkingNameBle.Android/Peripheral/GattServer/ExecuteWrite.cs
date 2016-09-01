using Android.Bluetooth;

namespace WorkingNameBle.Android.Peripheral.GattServer
{
    public class ExecuteWrite
    {
        public ExecuteWrite(BluetoothDevice bluetoothDevice, int requestId, bool execute)
        {
            BluetoothDevice = bluetoothDevice;
            RequestId = requestId;
            Execute = execute;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public int RequestId { get; }
        public bool Execute { get; }
    }
}