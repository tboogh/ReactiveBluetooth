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
        IBluetoothAbstractFactory Factory { get; }
        IObservable<ManagerState> Init(IScheduler scheduler = null);

        void Shutdown();

        IObservable<bool> StartAdvertising(AdvertisingOptions advertisingOptions, IList<IService> services);

        IObservable<bool> AddService(IService service);
        void RemoveService(IService service);
        void RemoveAllServices();

        bool SendResponse(IAttRequest request, int offset, byte[] value);
    }
}
