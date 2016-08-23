using System;
using Android.Bluetooth;

namespace WorkingNameBle.Android
{
    public class BleGattCallback : BluetoothGattCallback
    {
        public Action<BluetoothGatt, GattStatus, ProfileState> ConnectionStateChange;
        public Action<BluetoothGatt, GattStatus> ServicesDiscovered;




        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicChanged(gatt, characteristic);
        }

        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);
        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);
            ConnectionStateChange?.Invoke(gatt, status, newState);
        }

        public override void OnDescriptorRead(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorRead(gatt, descriptor, status);
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, GattStatus status)
        {
            base.OnDescriptorWrite(gatt, descriptor, status);
        }

        public override void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, GattStatus status)
        {
            base.OnReadRemoteRssi(gatt, rssi, status);
        }

        public override void OnReliableWriteCompleted(BluetoothGatt gatt, GattStatus status)
        {
            base.OnReliableWriteCompleted(gatt, status);
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);
            ServicesDiscovered?.Invoke(gatt, status);
        }
    }
}