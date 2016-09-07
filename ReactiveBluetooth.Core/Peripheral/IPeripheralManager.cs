using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.Core.Peripheral
{
    public interface IPeripheralManager
    {

        IBluetoothAbstractFactory Factory { get; }
        IObservable<ManagerState> State();
        
        /// <summary>
        /// Starts device advertising, stops when obersavable is disposed
        /// </summary>
        /// <param name="advertisingOptions"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        IObservable<bool> Advertise(AdvertisingOptions advertisingOptions, IList<IService> services);

        IObservable<bool> AddService(IService service);
        void RemoveService(IService service);
        void RemoveAllServices();

        bool SendResponse(IAttRequest request, int offset, byte[] value);
    }
}
