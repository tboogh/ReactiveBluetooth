using CoreBluetooth;
using Foundation;

namespace WorkingNameBle.iOS.Central.CentralManagerDelegate
{
    public class DiscoveredPeripheral
    {
        public CBCentralManager CbCentralManager { get; }
        public CBPeripheral Peripheral { get; }
        public NSDictionary AdvertisementData { get; }
        public NSNumber Rssi { get; }

        public DiscoveredPeripheral(CBCentralManager cbCentralManager, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber rssi)
        {
            CbCentralManager = cbCentralManager;
            Peripheral = peripheral;
            AdvertisementData = advertisementData;
            Rssi = rssi;
        }
    }
}