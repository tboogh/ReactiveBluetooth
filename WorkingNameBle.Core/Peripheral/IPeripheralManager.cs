using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Core.Peripheral
{
    public interface IPeripheralManager
    {
        ManagerState State { get; }

        IObservable<ManagerState> Init(IScheduler scheduler = null);

        void Shutdown();

        void StartAdvertising(AdvertisingOptions advertisingOptions);
    }
}
