using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public class CharacteristicReadRequest
    {
        public CharacteristicReadRequest(BluetoothDevice bluetoothDevice, int requestId, int offet, BluetoothGattCharacteristic characteristic)
        {
            BluetoothDevice = bluetoothDevice;
            RequestId = requestId;
            Offet = offet;
            Characteristic = characteristic;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public int RequestId { get; }
        public int Offet { get; }
        public BluetoothGattCharacteristic Characteristic { get; }
    }
}