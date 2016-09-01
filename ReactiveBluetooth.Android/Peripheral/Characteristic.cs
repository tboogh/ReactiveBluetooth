using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Bluetooth;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Android.Peripheral.GattServer;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(BluetoothGattCharacteristic characteristic, ServerCallback serverCallback)
        {
            GattCharacteristic = characteristic;

            ReadRequestObservable = serverCallback.CharacteristicReadRequestSubject
                .Where(x => x.Characteristic.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Offet, null, request.RequestId, request.BluetoothDevice));

            WriteRequestObservable = serverCallback.CharacteristicWriteRequestSubject
                .Where(x => x.Characteristic.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Offset, request.Value, request.RequestId, request.BluetoothDevice));
        }

        public BluetoothGattCharacteristic GattCharacteristic { get; }
        public Guid Uuid => Guid.Parse(GattCharacteristic.Uuid.ToString());
        public CharacteristicProperty Properties => GattCharacteristic.Properties.ToCharacteristicProperty();
        public CharacteristicPermission Permissions => GattCharacteristic.Permissions.ToCharacteristicPermission();
        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }
    }
}