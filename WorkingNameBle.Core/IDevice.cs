using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingNameBle.Core
{
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Disconnecting
    }

    public interface IDevice
    {
        string Name { get;  }

        ConnectionState State { get; }


        IObservable<IList<IService>> DiscoverServices();
    }
}
