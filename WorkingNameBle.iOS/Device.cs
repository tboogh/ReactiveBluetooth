using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS
{
    public class Device : IDevice
    {
        public Device(CBPeripheral nativeDevice)
        {
            NativeDevice = nativeDevice;
            Name = nativeDevice.Name;
        }

        public CBPeripheral NativeDevice { get; }

        public string Name { get; }

        public ConnectionState State => (ConnectionState) NativeDevice.State;

        public IObservable<IList<IService>> DiscoverServices()
        {
            var observable = Observable.FromEventPattern<EventHandler<NSErrorEventArgs>, NSErrorEventArgs>(eh => NativeDevice.DiscoveredService += eh, eh => NativeDevice.DiscoveredService -= eh);
            NativeDevice.DiscoverServices();
            return observable.Select(x =>
            {
                return NativeDevice.Services.Select(cbService => new Service(cbService, NativeDevice))
                    .Cast<IService>()
                    .ToList();
            });
        }
    }
}
