using System.Reactive.Subjects;
using CoreBluetooth;
using Foundation;

namespace WorkingNameBle.iOS.Central.CentralManagerDelegate
{
    public class CentralManagerDelegate : CBCentralManagerDelegate
    {
        public Subject<CBCentralManagerState> StateUpdatedSubject { get; }
        public Subject<ConnectedPeripheral> ConnectedPeripheralSubject { get; }

        public Subject<ConnectionStatePeripheral> DisconnectedPeripheralSubject { get; }
        public Subject<DiscoveredPeripheral> DiscoveredPeriperhalSubject { get; }
        public Subject<ConnectionStatePeripheral> FailedToConnectPeripheralSubject { get; }
        public Subject<RetrievedPeripherals> RetrievedConnectedPeripheralsSubject { get; }
        public Subject<RetrievedPeripherals> RetrievedPeripheralsSubject { get; }

        public CentralManagerDelegate()
        {
            StateUpdatedSubject = new Subject<CBCentralManagerState>();
            ConnectedPeripheralSubject = new Subject<ConnectedPeripheral>();
            DisconnectedPeripheralSubject = new Subject<ConnectionStatePeripheral>();
            DiscoveredPeriperhalSubject = new Subject<DiscoveredPeripheral>();
            FailedToConnectPeripheralSubject = new Subject<ConnectionStatePeripheral>();
            RetrievedConnectedPeripheralsSubject = new Subject<RetrievedPeripherals>();
            RetrievedPeripheralsSubject = new Subject<RetrievedPeripherals>();
        }

        public override void UpdatedState(CBCentralManager central)
        {
            StateUpdatedSubject?.OnNext(central.State);
        }

        public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            ConnectedPeripheralSubject?.OnNext(new ConnectedPeripheral(central, peripheral));
        }

        public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            DisconnectedPeripheralSubject?.OnNext(new ConnectionStatePeripheral(central, peripheral, error));
        }

        public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            DiscoveredPeriperhalSubject?.OnNext(new DiscoveredPeripheral(central, peripheral, advertisementData, RSSI)); 
        }

        public override void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            FailedToConnectPeripheralSubject?.OnNext(new ConnectionStatePeripheral(central, peripheral, error));
        }

        public override void RetrievedConnectedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            RetrievedConnectedPeripheralsSubject?.OnNext(new RetrievedPeripherals(central, peripherals));
        }

        public override void RetrievedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            RetrievedPeripheralsSubject?.OnNext(new RetrievedPeripherals(central, peripherals));
        }
    }
}