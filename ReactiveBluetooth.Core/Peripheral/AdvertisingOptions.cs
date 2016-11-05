using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core.Peripheral
{
    public class AdvertisingOptions
    {
        /// <summary>
        /// The Service UUIDs to include in the advertisment data.
        /// </summary>
        public List<Guid> ServiceUuids { get; set; }
        /// <summary>
        /// The localname of the device to be included in the advertisment data.
        /// </summary>
        public string LocalName { get; set; }

        /// <summary>
        /// The power mode to to perform advertising at. Only used on Android devices
        /// </summary>
        public AdvertiseMode AdvertiseMode { get; set; }

        /// <summary>
        /// The Tx power level to advertise at. Only used on Android devices
        /// </summary>
        public AdvertiseTx AdvertiseTx { get; set; }
    }
}
