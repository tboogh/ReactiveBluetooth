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
            DiscoveredCharacteristicSubject = new Subject<PeripheralServiceInfo>();
            DiscoveredDescriptorSubject = new Subject<PeripheralCharacteristicInfo>();
            DiscoveredIncludedServiceSubject = new Subject<PeripheralServiceInfo>();
            DiscoveredServiceSubject = new Subject<PeripheralInfo>();
            InvalidatedServiceSubject = new Subject<PeripheralInfo>();
            ModifiedServicesSubject = new Subject<ModifiedServices>();
            RssiReadSubject = new Subject<RssiRead>();
            RssiUpdatedSubject = new Subject<PeripheralInfo>();
            UpdatedCharacterteristicValueSubject = new Subject<PeripheralCharacteristicInfo>();
            UpdatedNameSubject = new Subject<PeripheralInfo>();
            UpdatedNotificationStateSubject = new Subject<PeripheralCharacteristicInfo>();
            UpdatedValueSubject = new Subject<PeripheralDescriptorInfo>();
            WroteCharacteristicValueSubject = new Subject<PeripheralCharacteristicInfo>();
            WroteDescriptorValueSubject = new Subject<PeripheralDescriptorInfo>();
        }

        public Subject<PeripheralServiceInfo> DiscoveredCharacteristicSubject { get; }
        public Subject<PeripheralCharacteristicInfo> DiscoveredDescriptorSubject { get; }
        public Subject<PeripheralServiceInfo> DiscoveredIncludedServiceSubject { get; }
        public Subject<PeripheralInfo> DiscoveredServiceSubject { get; }
        public Subject<PeripheralInfo> InvalidatedServiceSubject { get; }
        public Subject<ModifiedServices> ModifiedServicesSubject { get; }
        public Subject<RssiRead> RssiReadSubject { get; }
        public Subject<PeripheralInfo> RssiUpdatedSubject { get; }
        public Subject<PeripheralCharacteristicInfo> UpdatedCharacterteristicValueSubject { get; }
        public Subject<PeripheralInfo> UpdatedNameSubject { get; }
        public Subject<PeripheralCharacteristicInfo> UpdatedNotificationStateSubject { get; }
        public Subject<PeripheralDescriptorInfo> UpdatedValueSubject { get; }
        public Subject<PeripheralCharacteristicInfo> WroteCharacteristicValueSubject { get; }
        public Subject<PeripheralDescriptorInfo> WroteDescriptorValueSubject { get; }

        public override void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError error)
        {
            DiscoveredCharacteristicSubject?.OnNext(new PeripheralServiceInfo(peripheral, service, error));
        }

        public override void DiscoveredDescriptor(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            DiscoveredDescriptorSubject?.OnNext(new PeripheralCharacteristicInfo(peripheral, characteristic, error));
        }

        public override void DiscoveredIncludedService(CBPeripheral peripheral, CBService service, NSError error)
        {
            DiscoveredIncludedServiceSubject?.OnNext(new PeripheralServiceInfo(peripheral, service, error));
        }

        public override void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            DiscoveredServiceSubject?.OnNext(new PeripheralInfo(peripheral, error));
        }

        public override void InvalidatedService(CBPeripheral peripheral)
        {
            InvalidatedServiceSubject?.OnNext(new PeripheralInfo(peripheral));
        }

        public override void ModifiedServices(CBPeripheral peripheral, CBService[] services)
        {
            ModifiedServicesSubject?.OnNext(new ModifiedServices(peripheral, services));
        }

        public override void RssiRead(CBPeripheral peripheral, NSNumber rssi, NSError error)
        {
            RssiReadSubject?.OnNext(new RssiRead(peripheral, rssi, error));
        }

        public override void RssiUpdated(CBPeripheral peripheral, NSError error)
        {
            RssiUpdatedSubject?.OnNext(new PeripheralInfo(peripheral, error));
        }

        public override void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            UpdatedCharacterteristicValueSubject?.OnNext(new PeripheralCharacteristicInfo(peripheral, characteristic, error));
        }

        public override void UpdatedName(CBPeripheral peripheral)
        {
            UpdatedNameSubject?.OnNext(new PeripheralInfo(peripheral));
        }

        public override void UpdatedNotificationState(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            UpdatedNotificationStateSubject?.OnNext(new PeripheralCharacteristicInfo(peripheral, characteristic, error));
        }

        public override void UpdatedValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError error)
        {
            UpdatedValueSubject?.OnNext(new PeripheralDescriptorInfo(peripheral, descriptor, error));
        }

        public override void WroteCharacteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            WroteCharacteristicValueSubject?.OnNext(new PeripheralCharacteristicInfo(peripheral, characteristic, error));
        }

        public override void WroteDescriptorValue(CBPeripheral peripheral, CBDescriptor descriptor, NSError error)
        {
            WroteDescriptorValueSubject?.OnNext(new PeripheralDescriptorInfo(peripheral, descriptor, error));
        }
    }
}