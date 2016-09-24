using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Central
{
    public interface IDevice
    {
        string Name { get;  }

        Guid Uuid { get; }

        ConnectionState State { get; }

        Task<IList<IService>> DiscoverServices(CancellationToken cancellationToken);

        IObservable<int> Rssi { get; }

        void UpdateRemoteRssi();

        Task<byte[]> ReadValue(ICharacteristic characteristic, CancellationToken cancellationToken);
    }
}
