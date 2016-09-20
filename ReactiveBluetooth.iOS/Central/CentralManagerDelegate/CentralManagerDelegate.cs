using System;
using System.Reactive.Subjects;
using CoreBluetooth;
using Foundation;

namespace ReactiveBluetooth.iOS.Central.CentralManagerDelegate
{
    public class CentralManagerDelegate : CBCentralManagerDelegate
    {
        public BehaviorSubject<CBCentralManagerState> StateUpdatedSubject { get; }

        public Subject<Tuple<CBCentralManager, CBPeripheral>> ConnectedPeripheralSubject { get; }
        public Subject<Tuple<CBCentralManager, CBPeripheral, NSError>> DisconnectedPeripheralSubject { get; }
        public Subject<Tuple<CBCentralManager, CBPeripheral, NSError>> FailedToConnectPeripheralSubject { get; }
        public Subject<Tuple<CBCentralManager, CBPeripheral, NSDictionary, NSNumber>> DiscoveredPeriperhalSubject { get; }
        public Subject<Tuple<CBCentralManager, CBPeripheral[]>> RetrievedConnectedPeripheralsSubject { get; }
        public Subject<Tuple<CBCentralManager, CBPeripheral[]>> RetrievedPeripheralsSubject { get; }

        public CentralManagerDelegate()
        {
            StateUpdatedSubject = new BehaviorSubject<CBCentralManagerState>(CBCentralManagerState.Unknown);
            ConnectedPeripheralSubject = new Subject<Tuple<CBCentralManager, CBPeripheral>>();
            DisconnectedPeripheralSubject =new Subject<Tuple<CBCentralManager, CBPeripheral, NSError>>();
            FailedToConnectPeripheralSubject = new Subject<Tuple<CBCentralManager, CBPeripheral, NSError>>();
            DiscoveredPeriperhalSubject = new Subject<Tuple<CBCentralManager, CBPeripheral, NSDictionary, NSNumber>>();
            RetrievedConnectedPeripheralsSubject = new Subject<Tuple<CBCentralManager, CBPeripheral[]>>();
            RetrievedPeripheralsSubject = new Subject<Tuple<CBCentralManager, CBPeripheral[]>>();
        }

        public override void UpdatedState(CBCentralManager central)
        {
            StateUpdatedSubject?.OnNext(central.State);
        }

        public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            ConnectedPeripheralSubject?.OnNext(new Tuple<CBCentralManager, CBPeripheral>(central, peripheral));
        }

        public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            DisconnectedPeripheralSubject?.OnNext(new Tuple<CBCentralManager, CBPeripheral, NSError>(central, peripheral, error));
        }

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            DiscoveredPeriperhalSubject?.OnNext(new Tuple<CBCentralManager, CBPeripheral, NSDictionary, NSNumber>(central, peripheral, advertisementData, RSSI)); 
        }

        public override void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            FailedToConnectPeripheralSubject?.OnNext(new Tuple<CBCentralManager, CBPeripheral, NSError>(central, peripheral, error));
        }

        public override void RetrievedConnectedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            RetrievedConnectedPeripheralsSubject?.OnNext(new Tuple<CBCentralManager, CBPeripheral[]>(central, peripherals));
        }

        public override void RetrievedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            RetrievedPeripheralsSubject?.OnNext(new Tuple<CBCentralManager, CBPeripheral[]>(central, peripherals));
        }
    }
}