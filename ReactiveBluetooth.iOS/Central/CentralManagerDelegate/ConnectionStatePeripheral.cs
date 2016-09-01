using CoreBluetooth;
using Foundation;

namespace ReactiveBluetooth.iOS.Central.CentralManagerDelegate
{
    public class ConnectionStatePeripheral
    {
        public CBCentralManager CbCentralManager { get; }
        public CBPeripheral Peripheral { get; }
        public NSError Error { get; }

        public ConnectionStatePeripheral(CBCentralManager cbCentralManager, CBPeripheral peripheral, NSError error)
        {
            CbCentralManager = cbCentralManager;
            Peripheral = peripheral;
            Error = error;
        }
    }
}