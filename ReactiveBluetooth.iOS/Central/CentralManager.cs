using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.iOS.Central
{
    public class CentralManager : ICentralManager
    {
        private readonly CBCentralManager _centralManager;
        private readonly CentralManagerDelegate.CentralManagerDelegate _centralManagerDelegate;

        public CentralManager()
        {
            _centralManagerDelegate = new CentralManagerDelegate.CentralManagerDelegate();
            _centralManager = new CBCentralManager(_centralManagerDelegate, null);
        }

        public IObservable<ManagerState> State()
        {
            return _centralManagerDelegate.StateUpdatedSubject.Select(x => (ManagerState) x);
        }

        public IObservable<IDevice> ScanForDevices(IList<Guid> serviceUuid = null)
        {
            CBUUID[] cbuuids = serviceUuid?.Select(x => CBUUID.FromString(x.ToString()))
                .ToArray();

            var scanObservable = Observable.Create<IDevice>(observer =>
            {
                _centralManager.ScanForPeripherals(cbuuids);
                return Disposable.Create(() => { _centralManager.StopScan(); });
            })
                .Publish()
                .RefCount();
            return scanObservable.Merge(_centralManagerDelegate.DiscoveredPeriperhalSubject.Select(x =>
            {
                Device device = new Device(x.Item2, x.Item4.Int32Value, new AdvertisementData(x.Item3));
                return device;
            }));
        }

        public IObservable<ConnectionState> Connect(IDevice device)
        {
            return Observable.Create<ConnectionState>(observer =>
            {
                _centralManager.ConnectPeripheral(((Device) device).Peripheral);
                return Disposable.Empty;
            })
                .Publish()
                .RefCount()
                .Merge(_centralManagerDelegate.ConnectedPeripheralSubject.Where(x => Equals(x.Item2.Identifier, ((Device) device).Peripheral.Identifier))
                    .Select(x => ConnectionState.Connected))
                .Merge(_centralManagerDelegate.DisconnectedPeripheralSubject.Where(x => Equals(x.Item2.Identifier, ((Device) device).Peripheral.Identifier))
                    .Select(x => ConnectionState.Disconnected));
        }

        public void Disconnect(IDevice device)
        {
            _centralManager.CancelPeripheralConnection(((Device)device).Peripheral);
        }

        public void Dispose()
        {
        }
    }
}