using CoreBluetooth;

namespace WorkingNameBle.iOS.Central.CentralManagerDelegate
{
    public class ConnectedPeripheral
    {
        public CBCentralManager CentralManager { get; }
        public CBPeripheral Peripheral { get; }

        public ConnectedPeripheral(CBCentralManager centralManager, CBPeripheral peripheral)
        {
            CentralManager = centralManager;
            Peripheral = peripheral;
        }
    }
}