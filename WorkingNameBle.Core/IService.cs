using System;
using System.Collections.Generic;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Core
{
    public interface IService
    {
        Guid Uuid { get; }
        ServiceType ServiceType{ get; }
    }
}
