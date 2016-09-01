using System;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Central
{
    public interface ICentralManager
    {
        ManagerState State { get; }

        IObservable<ManagerState> Init(IScheduler scheduler = null);
        void Shutdown();

        

        IObservable<IDevice> ScanForDevices();

        Task<bool> ConnectToDevice(IDevice device);

        Task DisconnectDevice(IDevice device);
    }
}