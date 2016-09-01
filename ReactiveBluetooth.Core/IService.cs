using System;
using System.Collections.Generic;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.Core
{
    public interface IService
    {
        Guid Uuid { get; }
        ServiceType ServiceType{ get; }
    }
}
