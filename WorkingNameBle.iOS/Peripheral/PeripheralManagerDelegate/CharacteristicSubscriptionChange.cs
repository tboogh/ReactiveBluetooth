using CoreBluetooth;

namespace WorkingNameBle.iOS.Peripheral.PeripheralManagerDelegate
{
    public class CharacteristicSubscriptionChange
    {
        public CharacteristicSubscriptionChange(CBPeripheralManager peripheralManager, CBCentral central, CBCharacteristic characteristic)
        {
            PeripheralManager = peripheralManager;
            Central = central;
            Characteristic = characteristic;
        }

        public CBPeripheralManager PeripheralManager { get; }
        public CBCentral Central { get; }
        public CBCharacteristic Characteristic { get; }
    }
}