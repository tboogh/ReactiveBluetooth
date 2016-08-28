using System;
using System.Reactive.Subjects;
using CoreBluetooth;
using Foundation;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.iOS.Peripheral
{
    public class PeripheralManagerDelegate : CBPeripheralManagerDelegate
    {
        public BehaviorSubject<ManagerState> StateUpdatedSubject;
        public BehaviorSubject<bool> AdvertisingStartedSubject;
        public PeripheralManagerDelegate()
        {
            StateUpdatedSubject = new BehaviorSubject<ManagerState>(ManagerState.PoweredOff);
            AdvertisingStartedSubject = new BehaviorSubject<bool>(false);
        }

        public override void AdvertisingStarted(CBPeripheralManager peripheral, NSError error)
        {
            if (error != null)
            {
                AdvertisingStartedSubject.OnError(new Exception(error.LocalizedDescription));
            }
            else
            {
                AdvertisingStartedSubject.OnNext(true);
            }
            
        }

        public override void CharacteristicSubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            
        }

        public override void CharacteristicUnsubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            
        }

        public override void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
        {
            
        }

        public override void ReadyToUpdateSubscribers(CBPeripheralManager peripheral)
        {
            
        }

        public override void ServiceAdded(CBPeripheralManager peripheral, CBService service, NSError error)
        {
            
        }

        public override void WriteRequestsReceived(CBPeripheralManager peripheral, CBATTRequest[] requests)
        {
            
        }

        public override void StateUpdated(CBPeripheralManager peripheral)
        {
            StateUpdatedSubject.OnNext((ManagerState)peripheral.State);
        }
    }
}