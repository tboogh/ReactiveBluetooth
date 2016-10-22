using System;
using System.Reactive.Subjects;
using Android.Bluetooth;
using ReactiveBluetooth.Core.Exceptions;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public class ServerCallback : BluetoothGattServerCallback, IServerCallback
    {
        public ServerCallback()
        {
            CharacteristicReadRequestSubject = new Subject<Tuple<BluetoothDevice, int, int, BluetoothGattCharacteristic>>();
            CharacteristicWriteRequestSubject = new Subject<Tuple<BluetoothDevice, int, BluetoothGattCharacteristic, bool, bool, int, byte[]>>();
            ConnectionStateChangedSubject = new Subject<Tuple<BluetoothDevice, ProfileState, ProfileState>>();
            DescriptorReadRequestSubject = new Subject<Tuple<BluetoothDevice, int, int, BluetoothGattDescriptor>>();
            DescriptorWriteRequestSubject = new Subject<Tuple<BluetoothDevice, int, BluetoothGattDescriptor, bool, bool, int, byte[]>>();
            ExecuteWriteSubject = new Subject<Tuple<BluetoothDevice, int, bool>>();
            MtuChangedSubject = new Subject<Tuple<BluetoothDevice, int>>();
            NotificationSentSubject = new Subject<Tuple<BluetoothDevice, GattStatus>>();
            ServiceAddedSubject = new Subject<Tuple<ProfileState, BluetoothGattService>>();
        }

        public Subject<Tuple<BluetoothDevice, int, int, BluetoothGattCharacteristic>> CharacteristicReadRequestSubject { get; }
        public Subject<Tuple<BluetoothDevice, int, BluetoothGattCharacteristic, bool, bool, int, byte[]>> CharacteristicWriteRequestSubject { get; }
        public Subject<Tuple<BluetoothDevice, ProfileState, ProfileState>> ConnectionStateChangedSubject { get; }
        public Subject<Tuple<BluetoothDevice, int, int, BluetoothGattDescriptor>> DescriptorReadRequestSubject { get; }
        public Subject<Tuple<BluetoothDevice, int, BluetoothGattDescriptor, bool, bool, int, byte[]>> DescriptorWriteRequestSubject { get; }
        public Subject<Tuple<BluetoothDevice, int, bool>> ExecuteWriteSubject { get; }
        public Subject<Tuple<BluetoothDevice, int>> MtuChangedSubject { get; }
        public Subject<Tuple<BluetoothDevice, GattStatus>> NotificationSentSubject { get; }
        public Subject<Tuple<ProfileState, BluetoothGattService>> ServiceAddedSubject { get; }

        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattCharacteristic characteristic)
        {
            CharacteristicReadRequestSubject?.OnNext(new Tuple<BluetoothDevice, int, int , BluetoothGattCharacteristic>(device, requestId, offset, characteristic));
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            CharacteristicWriteRequestSubject?.OnNext(new Tuple<BluetoothDevice, int, BluetoothGattCharacteristic, bool, bool, int, byte[]>(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value));
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            ConnectionStateChangedSubject?.OnNext(new Tuple<BluetoothDevice, ProfileState, ProfileState>(device, status, newState));
        }

        public override void OnDescriptorReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattDescriptor descriptor)
        {
            var uuid = descriptor.Uuid.ToString();
            DescriptorReadRequestSubject?.OnNext(new Tuple<BluetoothDevice, int, int, BluetoothGattDescriptor>(device, requestId, offset, descriptor));
        }

        public override void OnDescriptorWriteRequest(BluetoothDevice device, int requestId, BluetoothGattDescriptor descriptor, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            var uuid = descriptor.Uuid.ToString();
            DescriptorWriteRequestSubject?.OnNext(new Tuple<BluetoothDevice, int, BluetoothGattDescriptor, bool, bool, int, byte[]>(device, requestId, descriptor, preparedWrite, responseNeeded, offset, value));
        }

        public override void OnExecuteWrite(BluetoothDevice device, int requestId, bool execute)
        {
            ExecuteWriteSubject?.OnNext(new Tuple<BluetoothDevice, int, bool>(device, requestId, execute));
        }

        public override void OnMtuChanged(BluetoothDevice device, int mtu)
        {
            MtuChangedSubject?.OnNext(new Tuple<BluetoothDevice, int>(device, mtu));
        }

        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            NotificationSentSubject?.OnNext(new Tuple<BluetoothDevice, GattStatus>(device, status));
        }

        public override void OnServiceAdded(ProfileState status, BluetoothGattService service)
        {
            ServiceAddedSubject?.OnNext(new Tuple<ProfileState, BluetoothGattService>(status, service));
        }
    }
}