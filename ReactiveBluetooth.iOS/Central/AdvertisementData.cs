using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.iOS.Central
{
    public class AdvertisementData : IAdvertisementData
    {
        private readonly NSDictionary _advertisementData;

        public AdvertisementData(NSDictionary advertisementData)
        {
            _advertisementData = advertisementData;
        }

        public string LocalName
        {
            get { throw new NotImplementedException(); }
        }

        public int TxPowerLevel
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] ManufacturerData
        {
            get { throw new NotImplementedException(); }
        }

        public List<Guid> ServiceUuids
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<Guid, byte[]> ServiceData
        {
            get { throw new NotImplementedException(); }
        }
    }
}