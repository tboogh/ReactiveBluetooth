using System;
using System.Reactive.Subjects;
using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Central
{
    public class GattCallback : BluetoothGattCallback
    {
        public GattCallback()
        {
            CharacteristicChangedSubject = new Subject<Tuple<BluetoothGatt, BluetoothGattCharacteristic>>();
            ConnectionStateChange = new Subject<ProfileState>();
            CharacteristicReadSubject = new Subject<Tuple<BluetoothGatt, BluetoothGattCharacteristic, GattStatus>>();
            CharacteristicWriteSubject = new Subject<Tuple<BluetoothGatt, BluetoothGattCharacteristic, GattStatus>>();
            DescriptorReadSubject = new Subject<Tuple<BluetoothGatt, BluetoothGattDescriptor, GattStatus>>();
            DescriptorWriteSubject = new Subject<Tuple<BluetoothGatt, BluetoothGattDescriptor, GattStatus>>();
            ReadRemoteRssiSubject = new Subject<Tuple<BluetoothGatt, int, GattStatus>>();
            ReliableWriteCompletedSubject = new Subject<Tuple<BluetoothGatt, GattStatus>>();
            ServicesDiscovered = new Subject<Tuple<BluetoothGatt, GattStatus>>();
        }

        public Subject<Tuple<BluetoothGatt, BluetoothGattCharacteristic>> CharacteristicChangedSubject { get; }
        public Subject<ProfileState> ConnectionStateChange { get; }
        public Subject<Tuple<BluetoothGatt, BluetoothGattCharacteristic, GattStatus>> CharacteristicReadSubject { get; }
        public Subject<Tuple<BluetoothGatt, BluetoothGattCharacteristic, GattStatus>> CharacteristicWriteSubject { get; }
        public Subject<Tuple<BluetoothGatt, BluetoothGattDescriptor, GattStatus>> DescriptorReadSubject { get; }
        public Subject<Tuple<BluetoothGatt, BluetoothGattDescriptor, GattStatus>> DescriptorWriteSubject { get; }
        public Subject<Tuple<BluetoothGatt, int, GattStatus>> ReadRemoteRssiSubject { get; }
        public Subject<Tuple<BluetoothGatt, GattStatus>> ReliableWriteCompletedSubject { get; }
        public Subject<Tuple<BluetoothGatt, GattStatus>> ServicesDiscovered { get; }


        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            CharacteristicChangedSubject?.OnNext(new Tuple<BluetoothGatt, BluetoothGattCharacteristic>(gatt, characteristic));
        }

        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            CharacteristicReadSubject?.OnNext(new Tuple<BluetoothGatt, BluetoothGattCharacteristic, GattStatus>(gatt, characteristic, status));
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            CharacteristicWriteSubject?.OnNext(new Tuple<BluetoothGatt, BluetoothGattCharacteristic, GattStatus>(gatt, characteristic, status));
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            ConnectionStateChange?.OnNext(newState);
        }

        public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            DescriptorReadSubject?.OnNext(new Tuple<BluetoothGatt, BluetoothGattDescriptor, GattStatus>(gatt, descriptor, status));
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            DescriptorWriteSubject?.OnNext(new Tuple<BluetoothGatt, BluetoothGattDescriptor, GattStatus>(gatt, descriptor, status));
        }

        public override void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, GattStatus status)
        {
            ReadRemoteRssiSubject?.OnNext(new Tuple<BluetoothGatt, int, GattStatus>(gatt, rssi, status));
        }

        public override void OnReliableWriteCompleted(BluetoothGatt gatt, GattStatus status)
        {
            ReliableWriteCompletedSubject?.OnNext(new Tuple<BluetoothGatt, GattStatus>(gatt, status));
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            ServicesDiscovered?.OnNext(new Tuple<BluetoothGatt, GattStatus>(gatt, status));           
        }
    }
}