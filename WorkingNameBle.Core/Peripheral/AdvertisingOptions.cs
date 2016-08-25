using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingNameBle.Core.Peripheral
{
    public class AdvertisingOptions
    {
        public List<Guid> ServiceUuids { get;  }
        public string LocalName { get; }

        public AdvertisingOptions(string localName, List<Guid> serviceUuids)
        {
            ServiceUuids = serviceUuids;
            LocalName = localName;
        }
    }
}
