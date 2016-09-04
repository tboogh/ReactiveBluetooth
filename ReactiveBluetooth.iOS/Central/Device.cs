using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.iOS.Central
{
    public class Device : IDevice
    {
        private PeripheralDelegate.PeripheralDelegate _cbPeripheralDelegate;
        private int int32Value;

        public Device(CBPeripheral peripheral, int rssi)
        {
            Peripheral = peripheral;
            _cbPeripheralDelegate = new PeripheralDelegate.PeripheralDelegate();
            peripheral.Delegate = _cbPeripheralDelegate;

            var currentRssi = Observable.Return(rssi);
            var delegateRssi =_cbPeripheralDelegate.RssiUpdatedSubject.Select(x => x.Peripheral.RSSI.Int32Value);
            Rssi = currentRssi.Merge(delegateRssi);
        }


        public CBPeripheral Peripheral { get; }
        public Guid Uuid => Guid.Parse(Peripheral.Identifier.ToString());
        public string Name => Peripheral.Name;
        public ConnectionState State => (ConnectionState) Peripheral.State;
        public IObservable<int> Rssi { get; }

        public IObservable<IList<IService>> DiscoverServices()
        {
            return Observable.Create<IList<IService>>(observer =>
            {
                var discoverDisp = _cbPeripheralDelegate.DiscoveredServicesSubject.Select(x => x.Peripheral.Services).Subscribe(services =>
                {
                    IList<IService> list = services.Select(x => new Service(x, Peripheral))
                        .Cast<IService>().ToList();
                    observer.OnNext(list);
                    observer.OnCompleted();
                });

                Peripheral.DiscoverServices();

                return Disposable.Create(() =>
                {
                    discoverDisp.Dispose();
                });
            });
        }

        public void UpdateRemoteRssi()
        {
            Peripheral.ReadRSSI();
        }
    }
}