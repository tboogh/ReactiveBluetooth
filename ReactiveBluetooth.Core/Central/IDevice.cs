using System;
using System.Collections.Generic;

namespace ReactiveBluetooth.Core.Central
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

        Guid Uuid { get; }

        ConnectionState State { get; }


        IObservable<IList<IService>> DiscoverServices();
    }
}
