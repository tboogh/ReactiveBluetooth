using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public class CharacteristicWriteRequest
    {
        public CharacteristicWriteRequest(BluetoothDevice bluetoothDevice, int requestId, BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            BluetoothDevice = bluetoothDevice;
            RequestId = requestId;
            Characteristic = characteristic;
            PreparedWrite = preparedWrite;
            ResponseNeeded = responseNeeded;
            Offset = offset;
            Value = value;
        }

        public BluetoothDevice BluetoothDevice { get; }
        public int RequestId { get; }
        public BluetoothGattCharacteristic Characteristic { get; }
        public bool PreparedWrite { get; }
        public bool ResponseNeeded { get; }
        public int Offset { get; }
        public byte[] Value { get; }
    }
}