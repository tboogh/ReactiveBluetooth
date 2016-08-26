using System;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.iOS.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Guid Id { get; }
    }
}