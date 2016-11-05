using System;
using Android.Bluetooth.LE;

namespace ReactiveBluetooth.Android.Extensions
{
    public static class AdvertiseTxExtensions
    {
        public static AdvertiseTx ToAdvertiseTx(this Core.Types.AdvertiseTx advertiseTx)
        {
            switch (advertiseTx)
            {
                case Core.Types.AdvertiseTx.PowerMedium:
                    return AdvertiseTx.PowerMedium;
                case Core.Types.AdvertiseTx.PowerUltraLow:
                    return AdvertiseTx.PowerUltraLow;
                case Core.Types.AdvertiseTx.PowerLow:
                    return AdvertiseTx.PowerLow;
                case Core.Types.AdvertiseTx.PowerHigh:
                    return AdvertiseTx.PowerHigh;
                default:
                    throw new ArgumentOutOfRangeException(nameof(advertiseTx), advertiseTx, null);
            }
        }
    }
}