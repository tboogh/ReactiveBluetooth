using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Exceptions;

namespace ReactiveBluetooth.Core.Central
{
    public interface ICentralManager : IDisposable
    {
        IObservable<ManagerState> State();

        /// <summary>
        /// Starts a scan for BLE devices, stops scanning on dispose
        /// </summary>
        /// <returns></returns>
        IObservable<IDevice> ScanForDevices();

        /// <summary>
        /// Throws <see cref="FailedToConnectException"/> if connection fails
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        IObservable<ConnectionState> ConnectToDevice(IDevice device);

        /// <summary>
        /// Triggers the start of a service discovery. Subscribe to property <seealso cref="IDevice.DiscoverServices"/>> for discovery results
        /// </summary>
        /// <param name="device"></param>
        void DiscoverServices(IDevice device);

        /// <summary>
        /// Disconnect the device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        Task DisconnectDevice(IDevice device);
    }
}