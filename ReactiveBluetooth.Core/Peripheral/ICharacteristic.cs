using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Peripheral
{
    public interface ICharacteristic : Core.ICharacteristic
    {
        IObservable<IAttRequest> ReadRequestObservable { get; }
        IObservable<IAttRequest> WriteRequestObservable { get; }
    }
}
