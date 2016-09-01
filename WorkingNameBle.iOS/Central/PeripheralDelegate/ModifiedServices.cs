using CoreBluetooth;

namespace WorkingNameBle.iOS.Central.PeripheralDelegate
{
    public class ModifiedServices
    {
        public CBPeripheral Peripheral { get; }
        public CBService[] Services { get; }

        public ModifiedServices(CBPeripheral peripheral, CBService[] services)
        {
            Peripheral = peripheral;
            Services = services;
        }
    }
}