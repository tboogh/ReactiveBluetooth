using System;
using System.Linq;
using CoreBluetooth;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Types;
using ReactiveBluetooth.iOS.Extensions;

namespace ReactiveBluetooth.iOS.Central
{
    public class Characteristic : ICharacteristic
    {
        public Characteristic(CBCharacteristic characteristic)
        {
            NativeCharacteristic = characteristic;
        }

        public CBCharacteristic NativeCharacteristic { get; }
        public Guid Uuid => NativeCharacteristic.UUID.Uuid.ToGuid();
        public CharacteristicProperty Properties => NativeCharacteristic.Properties.ToCharacteristicProperty();

        public IDescriptor[] Descriptors => NativeCharacteristic.Descriptors?.Select(x => new Descriptor(x))
            .Cast<IDescriptor>()
            .ToArray();
    }
}