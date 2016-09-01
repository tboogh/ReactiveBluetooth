using CoreBluetooth;

namespace ReactiveBluetooth.iOS.Peripheral.PeripheralManagerDelegate
{
    public class ReadRequestReceived
    {
        public ReadRequestReceived(CBPeripheralManager peripheralManager, CBATTRequest attRequest)
        {
            PeripheralManager = peripheralManager;
            AttRequest = attRequest;
        }

        public CBPeripheralManager PeripheralManager { get; }
        public CBATTRequest AttRequest { get; }
    }
}