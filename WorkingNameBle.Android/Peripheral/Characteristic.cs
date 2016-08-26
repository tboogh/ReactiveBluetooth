using System;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.Android.Peripheral
{
    public class Characteristic : ICharacteristic
    {
        public Guid Id { get; }
    }
}