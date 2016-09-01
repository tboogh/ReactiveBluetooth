using Android.Bluetooth;

namespace WorkingNameBle.Android.Peripheral.GattServer
{
    public class ConnectionStateChange
    {
        public ConnectionStateChange(BluetoothDevice bluetoothDevice, ProfileState status, ProfileState newState)
        {
            BluetoothDevice = bluetoothDevice;
            Status = status;
            NewState = newState;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public ProfileState Status { get; }
        public ProfileState NewState { get; }
    }
}