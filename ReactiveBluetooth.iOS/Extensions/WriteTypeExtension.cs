using System;
using System.Collections.Generic;
using System.Text;
using CoreBluetooth;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.iOS.Extensions
{
    public static class WriteTypeExtension
    {
        public static CBCharacteristicWriteType ToCharacteristicWriteType(this WriteType writeType)
        {
            switch (writeType)
            {
                case WriteType.WithoutRespoonse:
                    return CBCharacteristicWriteType.WithoutResponse;
                case WriteType.WithResponse:
                    return CBCharacteristicWriteType.WithResponse;
                default:
                    throw new ArgumentOutOfRangeException(nameof(writeType), writeType, null);
            }
        }
    }
}
