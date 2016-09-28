using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace ReactiveBluetooth.iOS.Central
{
    public class Device : IDevice
    {
        private readonly PeripheralDelegate.PeripheralDelegate _cbPeripheralDelegate;

        public Device(CBPeripheral peripheral, int rssi, IAdvertisementData advertisementData)
        {
            AdvertisementData = advertisementData;
            Peripheral = peripheral;
            _cbPeripheralDelegate = new PeripheralDelegate.PeripheralDelegate();
            peripheral.Delegate = _cbPeripheralDelegate;

            var currentRssi = Observable.Return(rssi);
            var delegateRssi = _cbPeripheralDelegate.RssiUpdatedSubject.Select(x => x.Item1.RSSI.Int32Value);
            Rssi = currentRssi.Merge(delegateRssi);
        }

        public CBPeripheral Peripheral { get; }

        public Guid Uuid => Guid.Parse(Peripheral.Identifier.ToString());

        public string Name => Peripheral.Name;
        public ConnectionState State => (ConnectionState) Peripheral.State;
        public IAdvertisementData AdvertisementData { get; }
        public IObservable<int> Rssi { get; }

        public Task<IList<IService>> DiscoverServices(CancellationToken cancellationToken)
        {
            if (Peripheral.Services != null)
            {
                return
                    Task.FromResult<IList<IService>>(
                        Peripheral.Services.Select(y => new Service(y, Peripheral, _cbPeripheralDelegate))
                            .Cast<IService>()
                            .ToList());
            }
            return Observable.FromEvent<IList<IService>>(action =>
            {
                Peripheral.DiscoverServices();
            }, _ => { })
                .Merge(
                    _cbPeripheralDelegate.DiscoveredServicesSubject.Select(
                        x =>
                        {
                            return x.Item1.Services.Select(y => new Service(y, Peripheral, _cbPeripheralDelegate))
                                .Cast<IService>()
                                .ToList();
                        }))
                .Take(1)
                .ToTask(cancellationToken);
        }

        public void UpdateRemoteRssi()
        {
            Peripheral.ReadRSSI();
        }

        public Task<byte[]> ReadValue(ICharacteristic characteristic, CancellationToken cancellationToken)
        {
            CBCharacteristic cbCharacteristic = ((Characteristic) characteristic).NativeCharacteristic;
            var observable = _cbPeripheralDelegate.UpdatedCharacterteristicValueSubject.FirstAsync(x =>
            {
                bool perphEqual = x.Item1.Identifier.ToString() == Peripheral.Identifier.ToString();
                bool chgarEqual = x.Item2.UUID.Uuid == cbCharacteristic.UUID.Uuid;
                return perphEqual && chgarEqual;
            }).Select(x => x.Item2.Value?.ToArray());

            return Observable.FromEvent<byte[]>(action => { Peripheral.ReadValue(cbCharacteristic); }, action => { }).Merge(observable).FirstAsync().ToTask(cancellationToken);
        }

        public Task WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}