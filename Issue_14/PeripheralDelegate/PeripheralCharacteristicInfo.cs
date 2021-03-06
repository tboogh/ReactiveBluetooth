﻿using CoreBluetooth;
using Foundation;

namespace Issue_14.PeripheralDelegate
{
    public class PeripheralCharacteristicInfo
    {
        public CBPeripheral Peripheral { get; }
        public CBCharacteristic Characteristic { get; }
        public NSError Error { get; }

        public PeripheralCharacteristicInfo(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            Peripheral = peripheral;
            Characteristic = characteristic;
            Error = error;
        }
    }
}