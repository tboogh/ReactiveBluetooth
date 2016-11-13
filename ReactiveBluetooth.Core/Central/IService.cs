using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Central
{
    public interface IService : Core.IService
    {
        Task<IList<ICharacteristic>> DiscoverCharacteristics(CancellationToken cancellationToken);
        Task<IList<IService>> DiscoverIncludedServices(CancellationToken cancellationToken, IList<Guid> serviceUuids = null);
    }
}
