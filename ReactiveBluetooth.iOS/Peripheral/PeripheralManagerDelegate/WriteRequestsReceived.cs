using CoreBluetooth;

namespace ReactiveBluetooth.iOS.Peripheral.PeripheralManagerDelegate
{
    public class WriteRequestsReceived
    {
        public WriteRequestsReceived(CBPeripheralManager peripheralManager, CBATTRequest[] requests)
        {
            PeripheralManager = peripheralManager;
            Requests = requests;
        }
        
        public CBPeripheralManager PeripheralManager { get; }
        public CBATTRequest[] Requests { get; }
    }
}