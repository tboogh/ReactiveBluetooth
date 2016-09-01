using CoreBluetooth;
using Foundation;

namespace ReactiveBluetooth.iOS.Central.PeripheralDelegate
{
    public class PeripheralInfo
    {
        public CBPeripheral Peripheral { get; }
        public NSError Error { get;  }

        public PeripheralInfo(CBPeripheral peripheral)
        {
            Peripheral = peripheral;
        }

        public PeripheralInfo(CBPeripheral peripheral, NSError error)
        {
            Peripheral = peripheral;
            Error = error;
        }
    }
}