using System;
using System.Collections.Generic;

namespace WorkingNameBle.Core.Central
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
