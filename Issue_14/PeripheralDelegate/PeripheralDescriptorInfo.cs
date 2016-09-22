using CoreBluetooth;
using Foundation;

namespace Issue_14.PeripheralDelegate
{
    public class PeripheralDescriptorInfo
    {
        public CBPeripheral Peripheral { get; }
        public CBDescriptor Descriptor { get; }
        public NSError Error { get; }

        public PeripheralDescriptorInfo(CBPeripheral peripheral, CBDescriptor descriptor, NSError error)
        {
            Peripheral = peripheral;
            Descriptor = descriptor;
            Error = error;
        }
    }
}