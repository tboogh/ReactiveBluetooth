using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using WorkingNameBle.Core.Exceptions;

namespace WorkingNameBle.Core
{
    public interface IBluetoothService
    {
        void Init(IScheduler scheduler = null);
        void Shutdown();

        /// <summary>
        /// Throws <see cref="DiscoverDeviceException"/> if scanning is not supported
        /// </summary>
        /// <returns></returns>
        Task<bool> ReadyToDiscover();
        IObservable<IDevice> ScanForDevices();

        Task<bool> ConnectToDevice(IDevice device);
    }
}