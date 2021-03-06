using System;
using System.Reactive.Subjects;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;

namespace ReactiveBluetooth.iOS.Peripheral.PeripheralManagerDelegate
{
    public class PeripheralManagerDelegate : CBPeripheralManagerDelegate
    {
        public PeripheralManagerDelegate()
        {
            StateUpdatedSubject = new BehaviorSubject<CBPeripheralManagerState>(CBPeripheralManagerState.Unknown);
            AdvertisingStartedSubject = new BehaviorSubject<bool>(false);
            CharacteristicSubscribedSubject = new Subject<CharacteristicSubscriptionChange>();
            CharacteristicUnsubscribedSubject = new Subject<CharacteristicSubscriptionChange>();
            ReadyToUpdateSubsciberSubject = new Subject<CBPeripheralManager>();
            ReadRequestReceivedSubject = new Subject<ReadRequestReceived>();
            ServiceAddedSubject = new Subject<ServiceAdded>();
            WriteRequestsReceivedSubject = new Subject<WriteRequestsReceived>();
        }

        public BehaviorSubject<CBPeripheralManagerState> StateUpdatedSubject { get; }
        public BehaviorSubject<bool> AdvertisingStartedSubject { get; }
        public Subject<CharacteristicSubscriptionChange> CharacteristicSubscribedSubject { get; }
        public Subject<CharacteristicSubscriptionChange> CharacteristicUnsubscribedSubject { get; }
        public Subject<ReadRequestReceived> ReadRequestReceivedSubject { get; }
        public Subject<CBPeripheralManager> ReadyToUpdateSubsciberSubject { get; }
        public Subject<ServiceAdded> ServiceAddedSubject { get; }
        public Subject<WriteRequestsReceived> WriteRequestsReceivedSubject { get; }

        public override void AdvertisingStarted(CBPeripheralManager peripheral, NSError error)
        {
            if (error != null)
            {
                AdvertisingStartedSubject.OnError(new AdvertiseException(error.LocalizedDescription));
            }
            else
            {
                AdvertisingStartedSubject.OnNext(true);
            }
        }

        public override void CharacteristicSubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            CharacteristicSubscribedSubject?.OnNext(new CharacteristicSubscriptionChange(peripheral, central, characteristic));
        }

        public override void CharacteristicUnsubscribed(CBPeripheralManager peripheral, CBCentral central, CBCharacteristic characteristic)
        {
            CharacteristicUnsubscribedSubject?.OnNext(new CharacteristicSubscriptionChange(peripheral, central, characteristic));
        }

        public override void ReadRequestReceived(CBPeripheralManager peripheral, CBATTRequest request)
        {
            ReadRequestReceivedSubject?.OnNext(new ReadRequestReceived(peripheral, request));
        }

        public override void ReadyToUpdateSubscribers(CBPeripheralManager peripheral)
        {
            ReadyToUpdateSubsciberSubject?.OnNext(peripheral);
        }

        public override void ServiceAdded(CBPeripheralManager peripheral, CBService service, NSError error)
        {
            if (error != null)
            {
                ServiceAddedSubject?.OnError(new AddServiceException(error.LocalizedDescription));
            }
            ServiceAddedSubject?.OnNext(new ServiceAdded(peripheral, service, error));
        }

        public override void WriteRequestsReceived(CBPeripheralManager peripheral, CBATTRequest[] requests)
        {
            WriteRequestsReceivedSubject?.OnNext(new WriteRequestsReceived(peripheral, requests));
        }

        public override void StateUpdated(CBPeripheralManager peripheralManager)
        {
            StateUpdatedSubject?.OnNext(peripheralManager.State);
        }
    }
}