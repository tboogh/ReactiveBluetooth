using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using CoreBluetooth;
using Foundation;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.iOS.Central
{
    public class Device : IDevice
    {
        public Device(CBPeripheral peripheral)
        {
            Peripheral = peripheral;
            Name = peripheral.Name;
        }

        public CBPeripheral Peripheral { get; }

        public string Name { get; }

        public ConnectionState State => (ConnectionState) Peripheral.State;

        public IObservable<IList<IService>> DiscoverServices()
        {
            var observable = Observable.FromEventPattern<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(eh => Peripheral.DiscoveredService += eh, eh => Peripheral.DiscoveredService -= eh);
            Peripheral.DiscoverServices();
            return observable.Select(x =>
            {
                return Peripheral.Services.Select(cbService => new Service(cbService, Peripheral))
                    .Cast<IService>()
                    .ToList();
            });
        }
    }
}
