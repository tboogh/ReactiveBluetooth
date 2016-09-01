using CoreBluetooth;
using Foundation;

namespace WorkingNameBle.iOS.Central.PeripheralDelegate
{
    public class RssiRead
    {
        public CBPeripheral Peripheral { get; }
        public NSNumber Rssi { get; }
        public NSError Error { get; }

        public RssiRead(CBPeripheral peripheral, NSNumber rssi, NSError error)
        {
            Peripheral = peripheral;
            Rssi = rssi;
            Error = error;
        }
    }
}