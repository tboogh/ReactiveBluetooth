using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace ReactiveBluetooth.iOS.Central
{
    public class Device : IDevice
    {
        private readonly PeripheralDelegate.PeripheralDelegate _cbPeripheralDelegate;

        public Device(CBPeripheral peripheral, int rssi)
        {
            Peripheral = peripheral;
            _cbPeripheralDelegate = new PeripheralDelegate.PeripheralDelegate();
            peripheral.Delegate = _cbPeripheralDelegate;

            var currentRssi = Observable.Return(rssi);
            var delegateRssi = _cbPeripheralDelegate.RssiUpdatedSubject.Select(x => x.Peripheral.RSSI.Int32Value);
            Rssi = currentRssi.Merge(delegateRssi);
        }

        public CBPeripheral Peripheral { get; }
        public Guid Uuid => Guid.Parse(Peripheral.Identifier.ToString());
        public string Name => Peripheral.Name;
        public ConnectionState State => (ConnectionState) Peripheral.State;
        public IObservable<int> Rssi { get; }

        public Task<IList<IService>> DiscoverServices()
        {
            return Observable.FromEvent<IList<IService>>(action => { Peripheral.DiscoverServices(); }, _ => { })
                .Merge(
                    _cbPeripheralDelegate.DiscoveredServicesSubject.Select(
                        x => x.Peripheral.Services.Select(y => new Service(y, Peripheral))
                            .Cast<IService>()
                            .ToList()))
                .Take(1)
                .ToTask();
        }

        public void UpdateRemoteRssi()
        {
            Peripheral.ReadRSSI();
        }

        public IObservable<byte[]> ReadValue(ICharacteristic characteristic)
        {
            throw new NotImplementedException();
        }
    }
}