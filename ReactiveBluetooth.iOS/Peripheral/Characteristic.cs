using System;
using System.Linq;
using System.Reactive.Linq;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.iOS.Extensions;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(CBMutableCharacteristic mutableCharacteristic, PeripheralManagerDelegate.PeripheralManagerDelegate peripheralDelegate)
        {
            MutableCharacteristic = mutableCharacteristic;
            ReadRequestObservable = peripheralDelegate.ReadRequestReceivedSubject.Where(x => x.AttRequest.Characteristic.UUID == MutableCharacteristic.UUID).Select(x => new AttRequest(this, x.AttRequest));
            WriteRequestObservable = peripheralDelegate.WriteRequestsReceivedSubject.SelectMany(received => received.Requests.Select(x => new AttRequest(this, x)).ToObservable()).Where(x => x.Characteristic.Uuid == Uuid);
        }

        public Guid Uuid => Guid.Parse(MutableCharacteristic.UUID.ToString());
        public CharacteristicProperty Properties => MutableCharacteristic.Properties.ToCharacteristicProperty();
        public CharacteristicPermission Permissions => MutableCharacteristic.Permissions.ToCharacteristicPermission();
        public CBMutableCharacteristic MutableCharacteristic { get; }
        public IObservable<IAttRequest> ReadRequestObservable { get; }
        public IObservable<IAttRequest> WriteRequestObservable { get; }

        public void Dispose()
        {
            
        }
    }
}