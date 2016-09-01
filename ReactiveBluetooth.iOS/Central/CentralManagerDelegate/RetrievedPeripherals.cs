using CoreBluetooth;

namespace ReactiveBluetooth.iOS.Central.CentralManagerDelegate
{
    public class RetrievedPeripherals
    {
        public CBCentralManager CbCentralManager { get; }
        public CBPeripheral[] Peripherals { get; }

        public RetrievedPeripherals(CBCentralManager cbCentralManager, CBPeripheral[] peripherals)
        {
            CbCentralManager = cbCentralManager;
            Peripherals = peripherals;
        }
    }
}