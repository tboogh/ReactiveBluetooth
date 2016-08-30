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
        private readonly Subject<IAttRequest> _readRequestSubject = new Subject<IAttRequest>();
        private readonly Subject<IAttRequest> _writeRequestSubject = new Subject<IAttRequest>();

        public Characteristic(BluetoothGattCharacteristic characteristic, ServerCallback serverCallback)
        {
            GattCharacteristic = characteristic;
            if (Properties.HasFlag(CharacteristicProperty.Read))
            {
                serverCallback.CharacteristicReadRequestSubject.Where(x => x.Characteristic.Uuid == characteristic.Uuid).Subscribe(request =>
                {
                    _readRequestSubject.OnNext(new AttRequest(this, request.Offet, null, request.RequestId, request.BluetoothDevice));
                });
            }
            if (Properties.HasFlag(CharacteristicProperty.Write))
            {
                serverCallback.CharacteristicWriteRequestSubject.Where(x => x.Characteristic.Uuid == characteristic.Uuid)
                    .Subscribe(request =>
                    {
                        _writeRequestSubject.OnNext(new AttRequest(this, request.Offset, request.Value, request.RequestId, request.BluetoothDevice));
                    });
            }
        }

        public BluetoothGattCharacteristic GattCharacteristic { get; }
        public Guid Uuid => Guid.Parse(GattCharacteristic.Uuid.ToString());
        public CharacteristicProperty Properties => GattCharacteristic.Properties.ToCharacteristicProperty();
        public CharacteristicPermission Permissions => GattCharacteristic.Permissions.ToCharacteristicPermission();
        public IObservable<IAttRequest> ReadRequestObservable => _readRequestSubject;
        public IObservable<IAttRequest> WriteRequestObservable => _writeRequestSubject;

        public void Dispose()
        {
            _readRequestSubject.Dispose();
            _writeRequestSubject.Dispose();
        }
    }
}