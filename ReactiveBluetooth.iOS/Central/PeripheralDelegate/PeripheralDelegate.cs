using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Security.Policy;
using System.Text;
using CoreBluetooth;
using Foundation;

namespace ReactiveBluetooth.iOS.Central.PeripheralDelegate
{
    public class PeripheralDelegate : CBPeripheralDelegate
    {
        public PeripheralDelegate()
        {
            DiscoveredCharacteristicsSubject = new Subject<Tuple<CBPeripheral, CBService, NSError>>();
            DiscoveredDescriptorsSubject = new Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>>();
            DiscoveredIncludedServicesSubject = new Subject<Tuple<CBPeripheral, CBService, NSError>>();
            DiscoveredServicesSubject = new Subject<Tuple<CBPeripheral, NSError>>();
            InvalidatedServiceSubject = new Subject<CBPeripheral>();
            ModifiedServicesSubject = new Subject<Tuple<CBPeripheral, CBService[]>>();
            RssiReadSubject = new Subject<Tuple<CBPeripheral, NSNumber, NSError>>();
            RssiUpdatedSubject = new Subject<Tuple<CBPeripheral, NSError>>();
            UpdatedCharacterteristicValueSubject = new Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>>();
            UpdatedNameSubject = new Subject<CBPeripheral>();
            UpdatedNotificationStateSubject = new Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>>();
            UpdatedValueSubject = new Subject<Tuple<CBPeripheral, CBDescriptor, NSError>>();
            WroteCharacteristicValueSubject = new Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>>();
            WroteDescriptorValueSubject = new Subject<Tuple<CBPeripheral, CBDescriptor, NSError>>();
        }

        public Subject<Tuple<CBPeripheral, CBService, NSError>> DiscoveredCharacteristicsSubject { get; }
        public Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>> DiscoveredDescriptorsSubject { get; }
        public Subject<Tuple<CBPeripheral, CBService, NSError>> DiscoveredIncludedServicesSubject { get; }
        public Subject<Tuple<CBPeripheral, NSError>> DiscoveredServicesSubject { get; }
        public Subject<CBPeripheral> InvalidatedServiceSubject { get; }
        public Subject<Tuple<CBPeripheral, CBService[]>> ModifiedServicesSubject { get; }
        public Subject<Tuple<CBPeripheral, NSNumber, NSError>> RssiReadSubject { get; }
        public Subject<Tuple<CBPeripheral, NSError>> RssiUpdatedSubject { get; }
        public Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>> UpdatedCharacterteristicValueSubject { get; }
        public Subject<CBPeripheral> UpdatedNameSubject { get; }
        public Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>> UpdatedNotificationStateSubject { get; }
        public Subject<Tuple<CBPeripheral, CBDescriptor, NSError>> UpdatedValueSubject { get; }
        public Subject<Tuple<CBPeripheral, CBCharacteristic, NSError>> WroteCharacteristicValueSubject { get; }
        public Subject<Tuple<CBPeripheral, CBDescriptor, NSError>> WroteDescriptorValueSubject { get; }

        public override void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError error)
        {
            DiscoveredCharacteristicsSubject?.OnNext(new Tuple<CBPeripheral, CBService, NSError>(peripheral, service, error));
        }

        public override void DiscoveredDescriptor(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            DiscoveredDescriptorsSubject?.OnNext(new Tuple<CBPeripheral, CBCharacteristic, NSError>(peripheral, characteristic, error));
        }

        public override void DiscoveredIncludedService(CBPeripheral peripheral, CBService service, NSError error)
        {
            DiscoveredIncludedServicesSubject?.OnNext(new Tuple<CBPeripheral, CBService, NSError>(peripheral, service, error));
        }

        public override void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            DiscoveredServicesSubject?.OnNext(new Tuple<CBPeripheral, NSError>(peripheral, error));
            DiscoveredServicesSubject?.OnCompleted();
        }

        public override void InvalidatedService(CBPeripheral peripheral)
        {
            InvalidatedServiceSubject?.OnNext(peripheral);
        }

        public override void ModifiedServices(CBPeripheral peripheral, CBService[] services)
        {
            ModifiedServicesSubject?.OnNext(new Tuple<CBPeripheral, CBService[]>(peripheral, services));
        }

        public override void RssiRead(CBPeripheral peripheral, NSNumber rssi, NSError error)
        {
            RssiReadSubject?.OnNext(new Tuple<CBPeripheral, NSNumber, NSError>(peripheral, rssi, error));
        }

        public override void RssiUpdated(CBPeripheral peripheral, NSError error)
        {
            RssiUpdatedSubject?.OnNext(new Tuple<CBPeripheral, NSError>(peripheral, error));
        }

        public override void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            UpdatedCharacterteristicValueSubject?.OnNext(new Tuple<CBPeripheral, CBCharacteristic, NSError>(peripheral, characteristic, error));
        }

        public override void UpdatedName(CBPeripheral peripheral)
        {
            UpdatedNameSubject?.OnNext(peripheral);
        }

        public override void UpdatedNotificationState(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            UpdatedNotificationStateSubject?.OnNext(new Tuple<CBPeripheral, CBCharacteristic, NSError>(peripheral, characteristic, error));
        }

        public override void UpdatedValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError error)
        {
            UpdatedValueSubject?.OnNext(new Tuple<CBPeripheral, CBDescriptor, NSError>(peripheral, descriptor, error));
        }

        public override void WroteCharacteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            WroteCharacteristicValueSubject?.OnNext(new Tuple<CBPeripheral, CBCharacteristic, NSError>(peripheral, characteristic, error));
        }

        public override void WroteDescriptorValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError error)
        {
            WroteDescriptorValueSubject?.OnNext(new Tuple<CBPeripheral, CBDescriptor, NSError>(peripheral, descriptor, error));
        }
    }
}