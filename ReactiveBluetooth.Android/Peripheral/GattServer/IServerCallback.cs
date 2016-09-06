using System.Reactive.Subjects;
using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public interface IServerCallback
    {
        Subject<CharacteristicReadRequest> CharacteristicReadRequestSubject { get; }
        Subject<CharacteristicWriteRequest> CharacteristicWriteRequestSubject { get; }
        Subject<ConnectionStateChange> ConnectionStateChangedSubject { get; }
        Subject<DescriptorReadRequest> DescriptorReadRequestSubject { get; }
        Subject<DescriptorWriteRequest> DescriptorWriteRequestSubject { get; }
        Subject<ExecuteWrite> ExecuteWriteSubject { get; }
        Subject<MtuChanged> MtuChangedSubject { get; }
        Subject<NotificationSent> NotificationSentSubject { get; }
        Subject<ServiceAdded> ServiceAddedSubject { get; }

        void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattCharacteristic characteristic);
        void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value);
        void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState);
        void OnDescriptorReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattDescriptor descriptor);
        void OnDescriptorWriteRequest(BluetoothDevice device, int requestId, BluetoothGattDescriptor descriptor, bool preparedWrite, bool responseNeeded, int offset, byte[] value);
        void OnExecuteWrite(BluetoothDevice device, int requestId, bool execute);
        void OnMtuChanged(BluetoothDevice device, int mtu);
        void OnNotificationSent(BluetoothDevice device, GattStatus status);
        void OnServiceAdded(ProfileState status, BluetoothGattService service);
    }
}