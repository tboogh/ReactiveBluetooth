using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Text;
using CoreBluetooth;
using CoreFoundation;
using Foundation;
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.iOS.Peripheral
{
    public class PeripheralManagerDelegate : CBPeripheralManagerDelegate
    {
        public Subject<ManagerState> StateUpdatedSubject;
        public Subject<bool> AdvertisingStartedSubject;
        public PeripheralManagerDelegate()
        {
            StateUpdatedSubject = new Subject<ManagerState>();
            AdvertisingStartedSubject = new Subject<bool>();
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

    public class PeripheralManager : IPeripheralManager
    {
        private CBPeripheralManager _peripheralManager;
        private PeripheralManagerDelegate _peripheralDelegate;

        public ManagerState State
        {
            get
            {
                if (_peripheralManager != null)
                    return (ManagerState) _peripheralManager.State;
                return ManagerState.PoweredOff;
            }
        }
        
        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            _peripheralDelegate = new PeripheralManagerDelegate();
            _peripheralManager = new CBPeripheralManager(_peripheralDelegate, DispatchQueue.MainQueue);

            return _peripheralDelegate.StateUpdatedSubject;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> StartAdvertising(AdvertisingOptions advertisingOptions)
        {
            // Check state
            StartAdvertisingOptions options = CreateAdvertisementOptions(advertisingOptions);
            _peripheralManager.StartAdvertising(options);

            return _peripheralDelegate.AdvertisingStartedSubject;
        }

        public StartAdvertisingOptions CreateAdvertisementOptions(AdvertisingOptions advertisingOptions)
        {
            return new StartAdvertisingOptions
            {
                LocalName = advertisingOptions.LocalName,
                ServicesUUID = advertisingOptions.ServiceUuids.Select(x => CBUUID.FromString(x.ToString())).ToArray()
            };
        }
    }
}
