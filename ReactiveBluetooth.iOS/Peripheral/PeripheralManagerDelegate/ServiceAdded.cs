using CoreBluetooth;
using Foundation;

namespace ReactiveBluetooth.iOS.Peripheral.PeripheralManagerDelegate
{
    public class ServiceAdded
    {
        public ServiceAdded(CBPeripheralManager peripheralManager, CBService service, NSError error)
        {
            PeripheralManager = peripheralManager;
            Service = service;
            Error = error;
        }

        public CBPeripheralManager PeripheralManager { get; }
        public CBService Service { get; }
        public NSError Error { get; }
    }
}