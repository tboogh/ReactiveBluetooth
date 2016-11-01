using System;
using System.Collections.Generic;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core
{
    /// <summary>
    /// Base interface for a service
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Unique UUID of characteristic
        /// </summary>
        Guid Uuid { get; }

        /// <summary>
        /// The type of <see cref="ServiceType"/>
        /// </summary>
        ServiceType ServiceType{ get; }
    }
}
