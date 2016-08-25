using System;
using System.Collections.Generic;

namespace WorkingNameBle.Core.Central
{
    public interface IService : Core.IService
    {
        IObservable<IList<ICharacteristic>> DiscoverCharacteristics();
    }
}
