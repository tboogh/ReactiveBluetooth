using System;
using System.Reactive.Subjects;
using Android.Bluetooth;

namespace ReactiveBluetooth.Android.Peripheral.GattServer
{
    public interface IServerCallback
    {
        /// <summary>
        /// Device, RequestId, Offset, Characteristic
        /// </summary>
        Subject<Tuple<BluetoothDevice, int, int, BluetoothGattCharacteristic>> CharacteristicReadRequestSubject { get; }
        Subject<Tuple<BluetoothDevice, int, BluetoothGattCharacteristic, bool, bool, int, byte[]>> CharacteristicWriteRequestSubject { get; }
        Subject<Tuple<BluetoothDevice, ProfileState, ProfileState>> ConnectionStateChangedSubject { get; }
        Subject<Tuple<BluetoothDevice, int, int, BluetoothGattDescriptor>> DescriptorReadRequestSubject { get; }
        Subject<Tuple<BluetoothDevice, int, BluetoothGattDescriptor, bool, bool, int, byte[]>> DescriptorWriteRequestSubject { get; }
        Subject<Tuple<BluetoothDevice, int, bool>> ExecuteWriteSubject { get; }
        Subject<Tuple<BluetoothDevice, int>> MtuChangedSubject { get; }
        Subject<Tuple<BluetoothDevice, GattStatus>> NotificationSentSubject { get; }
        Subject<Tuple<ProfileState, BluetoothGattService>> ServiceAddedSubject { get; }
    }
}