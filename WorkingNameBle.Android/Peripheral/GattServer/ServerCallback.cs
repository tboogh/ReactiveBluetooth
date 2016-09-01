using System.Reactive.Subjects;
using Android.Bluetooth;
using WorkingNameBle.Core.Exceptions;

namespace WorkingNameBle.Android.Peripheral.GattServer
{
    public class ServerCallback : BluetoothGattServerCallback
    {
        public ServerCallback()
        {
            CharacteristicReadRequestSubject = new Subject<CharacteristicReadRequest>();
            CharacteristicWriteRequestSubject = new Subject<CharacteristicWriteRequest>();
            ConnectionStateChangedSubject = new Subject<ConnectionStateChange>();
            DescriptorReadRequestSubject = new Subject<DescriptorReadRequest>();
            DescriptorWriteRequestSubject = new Subject<DescriptorWriteRequest>();
            ExecuteWriteSubject = new Subject<ExecuteWrite>();
            MtuChangedSubject = new Subject<MtuChanged>();
            NotificationSentSubject = new Subject<NotificationSent>();
            ServiceAddedSubject = new Subject<ServiceAdded>();
        }

        public Subject<CharacteristicReadRequest> CharacteristicReadRequestSubject { get; }
        public Subject<CharacteristicWriteRequest> CharacteristicWriteRequestSubject { get; }
        public Subject<ConnectionStateChange> ConnectionStateChangedSubject { get; }
        public Subject<DescriptorReadRequest> DescriptorReadRequestSubject { get; }
        public Subject<DescriptorWriteRequest> DescriptorWriteRequestSubject { get; }
        public Subject<ExecuteWrite> ExecuteWriteSubject { get; }
        public Subject<MtuChanged> MtuChangedSubject { get; }
        public Subject<NotificationSent> NotificationSentSubject { get; }
        public Subject<ServiceAdded> ServiceAddedSubject { get; }

        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattCharacteristic characteristic)
        {
            CharacteristicReadRequestSubject?.OnNext(new CharacteristicReadRequest(device, requestId, offset, characteristic));
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            CharacteristicWriteRequestSubject?.OnNext(new CharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value));
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            ConnectionStateChangedSubject?.OnNext(new ConnectionStateChange(device, status, newState));
        }

        public override void OnDescriptorReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattDescriptor descriptor)
        {
            DescriptorReadRequestSubject?.OnNext(new DescriptorReadRequest(device, requestId, descriptor));
        }

        public override void OnDescriptorWriteRequest(BluetoothDevice device, int requestId, BluetoothGattDescriptor descriptor, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            DescriptorWriteRequestSubject?.OnNext(new DescriptorWriteRequest(device, requestId, descriptor, preparedWrite, responseNeeded, offset, value));
        }

        public override void OnExecuteWrite(BluetoothDevice device, int requestId, bool execute)
        {
            ExecuteWriteSubject?.OnNext(new ExecuteWrite(device, requestId, execute));
        }

        public override void OnMtuChanged(BluetoothDevice device, int mtu)
        {
            MtuChangedSubject?.OnNext(new MtuChanged(device, mtu));
        }

        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            NotificationSentSubject?.OnNext(new NotificationSent(device, status));
        }

        public override void OnServiceAdded(ProfileState status, BluetoothGattService service)
        {
            if (status != 0)
            {
                ServiceAddedSubject?.OnError(new AddServiceException($"Could not add server. Error Code: {status}"));
            }
            ServiceAddedSubject?.OnNext(new ServiceAdded(status, service));
        }
    }
}