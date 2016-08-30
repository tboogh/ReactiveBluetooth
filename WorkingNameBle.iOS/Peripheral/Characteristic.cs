using System;
using CoreBluetooth;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Peripheral;
using ICharacteristic = WorkingNameBle.Core.Peripheral.ICharacteristic;

namespace WorkingNameBle.iOS.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(CBMutableCharacteristic mutableCharacteristic)
        {
            MutableCharacteristic = mutableCharacteristic;
        }

        public Guid Uuid { get; }
        public CharacteristicProperty Properties { get; }
        public CharacteristicPermission Permissions { get; }
        public CBMutableCharacteristic MutableCharacteristic { get; }
        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}