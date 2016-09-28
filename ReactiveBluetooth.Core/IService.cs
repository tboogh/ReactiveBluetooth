using System;
using System.Collections.Generic;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core
{
    public interface IService
    {
        Guid Uuid { get; }
        ServiceType ServiceType{ get; }
    }
}
