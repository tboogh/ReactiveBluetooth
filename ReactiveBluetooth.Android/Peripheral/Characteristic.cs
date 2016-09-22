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
        public Characteristic(BluetoothGattCharacteristic characteristic, IServerCallback serverCallback)
        {
            GattCharacteristic = characteristic;

            ReadRequestObservable = serverCallback.CharacteristicReadRequestSubject
                .Where(x => x.Item4.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Item3, null, request.Item2, request.Item1));

            WriteRequestObservable = serverCallback.CharacteristicWriteRequestSubject
                .Where(x => x.Item3.Uuid == characteristic.Uuid)
                .Select(request => new AttRequest(this, request.Item6, request.Item7, request.Item2, request.Item1));
        }

        public BluetoothGattCharacteristic GattCharacteristic { get; }
        public Guid Uuid => Guid.Parse(GattCharacteristic.Uuid.ToString());
        public CharacteristicProperty Properties => GattCharacteristic.Properties.ToCharacteristicProperty();
        public CharacteristicPermission Permissions => GattCharacteristic.Permissions.ToCharacteristicPermission();
        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }
    }
}