using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.Android.Central
{
    public class AdvertisementData : IAdvertisementData
    {
        private readonly ScanRecord _scanRecord;

        public AdvertisementData(ScanRecord scanRecord)
        {
            _scanRecord = scanRecord;
        }

        public string LocalName => _scanRecord?.DeviceName;
        public int TxPowerLevel => (int) _scanRecord.TxPowerLevel;

        public byte[] ManufacturerData
        {
            get
            {
                throw new NotImplementedException(); 
            }
        }

        public List<Guid> ServiceUuids => _scanRecord.ServiceUuids?.Select(x => Guid.Parse(x.Uuid.ToString())).ToList();

        public Dictionary<Guid, byte[]> ServiceData => _scanRecord.ServiceData?.ToDictionary(x => Guid.Parse(x.Key.Uuid.ToString()), x => x.Value);
    }
}