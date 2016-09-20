using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        Task<IList<IService>> DiscoverServices();

        IObservable<int> Rssi { get; }

        void UpdateRemoteRssi();

        IObservable<byte[]> ReadValue(ICharacteristic characteristic);
    }
}
