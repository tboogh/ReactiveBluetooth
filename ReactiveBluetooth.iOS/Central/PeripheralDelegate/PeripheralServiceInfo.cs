using CoreBluetooth;
using Foundation;

namespace ReactiveBluetooth.iOS.Central.PeripheralDelegate
{
    public class PeripheralServiceInfo
    {
        public CBPeripheral Peripheral { get; }
        public CBService Service { get; }
        public NSError Error { get; }

        public PeripheralServiceInfo(CBPeripheral peripheral, CBService service, NSError error)
        {
            Peripheral = peripheral;
            Service = service;
            Error = error;
        }
    }
}