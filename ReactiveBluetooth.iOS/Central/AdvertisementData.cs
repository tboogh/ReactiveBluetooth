using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Extensions;

namespace ReactiveBluetooth.iOS.Central
{
    public class AdvertisementData : IAdvertisementData
    {
        private readonly NSDictionary _advertisementData;

        public AdvertisementData(NSDictionary advertisementData)
        {
            _advertisementData = advertisementData;
        }

        public string LocalName => _advertisementData[CBAdvertisement.DataLocalNameKey]?.ToString();

        public int TxPowerLevel
        {
            get
            {
                NSNumber txPower = (NSNumber) _advertisementData[CBAdvertisement.DataTxPowerLevelKey];
                return txPower?.Int32Value ?? 0;
            }
        }

        public byte[] ManufacturerData
        {
            get
            {
                NSData data = (NSData)_advertisementData[CBAdvertisement.DataManufacturerDataKey];
                return data?.ToArray();
            }
        }

        public List<Guid> ServiceUuids
        {
            get
            {
                NSArray uuids = (NSArray) _advertisementData[CBAdvertisement.DataServiceUUIDsKey];
                List<Guid> guids = new List<Guid>();

                for (nuint i = 0; i < uuids.Count; ++i)
                {
                    CBUUID uuid = uuids.GetItem<CBUUID>(i);
                    guids.Add(uuid.Uuid.ToGuid());
                }

                return guids;
            }
        }

        public Dictionary<Guid, byte[]> ServiceData
        {
            get
            {
                NSDictionary<CBUUID, NSData> serviceData = (NSDictionary<CBUUID, NSData>) _advertisementData[CBAdvertisement.DataServiceDataKey];
                Dictionary<Guid, byte[]> dictionary = new Dictionary<Guid, byte[]>();
                foreach (var nsObject in serviceData)
                {
                    CBUUID cbuuid = (CBUUID) nsObject.Key;
                    NSData data = (NSData) nsObject.Value;
                    dictionary[cbuuid.Uuid.ToGuid()] = data.ToArray();
                }
                return dictionary;
            }
        }
    }
}