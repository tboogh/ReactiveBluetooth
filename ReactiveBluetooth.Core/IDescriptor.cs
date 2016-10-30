using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core
{
    /// <summary>
    /// Base interface for descriptors
    /// </summary>
    public interface IDescriptor
    {
        /// <summary>
        /// UUID of the descriptor
        /// </summary>
        Guid Uuid { get; }
    }
}
