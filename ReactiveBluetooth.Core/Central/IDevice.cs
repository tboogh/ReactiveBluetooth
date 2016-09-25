using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Types;

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

        Task<bool> WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken);
    }
}
