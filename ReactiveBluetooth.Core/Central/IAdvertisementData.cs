using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Central
{
    public interface IAdvertisementData
    {
        string LocalName { get; }
        int TxPowerLevel { get; }
        byte[] ManufacturerData { get; }
        List<Guid> ServiceUuids { get; }
        Dictionary<Guid, byte[]> ServiceData { get; }
    }
}
