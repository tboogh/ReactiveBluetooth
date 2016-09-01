using System;
using System.Collections.Generic;

namespace ReactiveBluetooth.Core.Central
{
    public interface IService : Core.IService
    {
        IObservable<IList<ICharacteristic>> DiscoverCharacteristics();
    }
}
