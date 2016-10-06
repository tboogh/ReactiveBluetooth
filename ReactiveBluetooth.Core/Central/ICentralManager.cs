using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Exceptions;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.Core.Central
{
    /// <summary>
    /// <see cref="ICentralManager"/> handles scanning for devices and connecting to discovered devices.
    /// Platform notes:
    ///     iOS: An <see cref="IDevice"/> that is disconnected from can not be reconnected to reliably. It needs to rediscovered using <see cref="ScanForDevices"/> before a new connection too guarantee that everything will work correctely
    /// </summary>
    public interface ICentralManager : IDisposable
    {
        /// <summary>
        /// The <see cref="ManagerState"/> changes of the central manager
        /// </summary>
        /// <returns></returns>
        IObservable<ManagerState> State();

        /// <summary>
        /// Starts a scan for BLE devices, stops scanning when the <see cref="IObservable{IDevice}"/> is completely disposed
        /// </summary>
        /// <returns>An observable with <see cref="IDevice"/> discovered over time</returns>
        IObservable<IDevice> ScanForDevices();

        /// <summary>
        /// Tries to connect to a <see cref="IDevice"/>
        /// Throws <see cref="FailedToConnectException"/> if connection fails
        /// </summary>
        /// <param name="device">The device to connect to</param>
        /// <returns></returns>
        IObservable<ConnectionState> ConnectToDevice(IDevice device);
    }
}