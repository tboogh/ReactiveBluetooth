using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Types
{
    public enum AdvertiseMode
    {
        /// <summary>
        /// Perform Bluetooth LE advertising in balanced power mode.
        /// </summary>
        Balanced,

        /// <summary>
        /// Perform Bluetooth LE advertising in low power mode.
        /// </summary>
        LowPower,

        /// <summary>
        /// Perform Bluetooth LE advertising in low latency, high power mode.
        /// </summary>
        LowLatency,
    }

    public enum AdvertiseTx
    {
        /// <summary>
        /// Advertise using medium TX power level.
        /// </summary>
        PowerMedium,

        /// <summary>
        /// Advertise using the lowest transmission (TX) power level.
        /// </summary>
        PowerUltraLow,

        /// <summary>
        /// Advertise using low TX power level.
        /// </summary>
        PowerLow,

        /// <summary>
        /// Advertise using high TX power level.
        /// </summary>
        PowerHigh,
    }
}