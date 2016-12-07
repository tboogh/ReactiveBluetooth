using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core.Central
{
    public interface IDevice
    {
        string Name { get;  }

        Guid Uuid { get; }

        IObservable<ConnectionState> ConnectionState { get; }

        IAdvertisementData AdvertisementData { get; }

        Task<IList<IService>> DiscoverServices(CancellationToken cancellationToken);

        IObservable<int> Rssi { get; }

        void UpdateRemoteRssi();

        Task<byte[]> ReadValue(ICharacteristic characteristic, CancellationToken cancellationToken);
        Task<byte[]> ReadValue(IDescriptor descriptor, CancellationToken cancellationToken);

        Task<bool> WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken);
        Task<bool> WriteValue(IDescriptor descriptor, byte[] value, CancellationToken cancellationToken);

        IObservable<byte[]> Notifications(ICharacteristic characteristic);
        Task<bool> StartNotifications(ICharacteristic characteristic, CancellationToken cancellationToken);
        Task<bool> StopNotifiations(ICharacteristic characteristic, CancellationToken cancellationToken);

        /// <summary>
        /// Request a connection parameter update.
        /// This is only implemented on android and will always return true on iOS
        /// </summary>
        /// <param name="priority">The desired priority</param>
        /// <returns>True if the operation was a success</returns>
        bool RequestConnectionPriority(ConnectionPriority priority);
    }
}
