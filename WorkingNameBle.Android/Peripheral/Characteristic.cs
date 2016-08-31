using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Bluetooth;
using WorkingNameBle.Android.Extensions;
using WorkingNameBle.Android.Peripheral.GattServer;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Peripheral;
using ICharacteristic = WorkingNameBle.Core.Peripheral.ICharacteristic;

namespace WorkingNameBle.Android.Peripheral
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