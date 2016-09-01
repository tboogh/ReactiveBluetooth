using Android.Bluetooth;

namespace WorkingNameBle.Android.Peripheral.GattServer
{
    public class NotificationSent
    {
        public NotificationSent(BluetoothDevice bluetoothDevice, GattStatus status)
        {
            BluetoothDevice = bluetoothDevice;
            Status = status;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public GattStatus Status { get; }
    }
}