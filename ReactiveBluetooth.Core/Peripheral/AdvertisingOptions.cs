using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Peripheral
{
    public class AdvertisingOptions
    {
        public List<Guid> ServiceUuids { get; set; }
        /// <summary>
        /// The localname of the device to be included in the advertisment data
        /// Warning: Android has issues with splitting the name into multiple packets so setting this often causes a data too large error
        /// </summary>
        public string LocalName { get; set; }
    }
}
