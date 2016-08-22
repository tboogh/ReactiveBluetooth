using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace WorkingNameBle.Core
{
    public interface IBluetoothService
    {
        void Init(IScheduler scheduler = null);
        void Shutdown();
        Task<bool> ReadyToDiscover();
        IObservable<IDevice> ScanForDevices();
    }
}