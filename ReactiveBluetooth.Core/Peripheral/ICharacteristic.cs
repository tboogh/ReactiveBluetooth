﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core.Peripheral
{
    public interface ICharacteristic : Core.ICharacteristic
    {
        IObservable<IAttRequest> ReadRequestObservable { get; }
        IObservable<IAttRequest> WriteRequestObservable { get; }

        CharacteristicPermission Permissions { get; }

        void AddDescriptor(IDescriptor descriptor);
    }
}
