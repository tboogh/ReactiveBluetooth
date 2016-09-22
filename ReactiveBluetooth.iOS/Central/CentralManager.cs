using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;

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
            return _centralManagerDelegate.StateUpdatedSubject.Select(x => (ManagerState)x);
        }

        public IObservable<IDevice> ScanForDevices()
        {
            var scanObservable = Observable.FromEvent<IDevice>(action => { _centralManager.ScanForPeripherals((CBUUID[])null); }, action => { _centralManager.StopScan(); });
            return scanObservable.Merge(_centralManagerDelegate.DiscoveredPeriperhalSubject.Select(x =>
            {
                Device device = new Device(x.Item2, x.Item4.Int32Value);
                return device;
            }));

        }

        public IObservable<ConnectionState> ConnectToDevice(IDevice device)
        {
            return Observable.FromEvent<ConnectionState>(action =>
            {
                _centralManager.ConnectPeripheral(((Device)device).Peripheral);
                action(device.State);
            }, action =>
            {
                _centralManager.CancelPeripheralConnection(((Device)device).Peripheral); 
            }).Merge(_centralManagerDelegate.ConnectedPeripheralSubject
                    .Where(x => Equals(x.Item2.Identifier, ((Device)device).Peripheral.Identifier))
                    .Select(x => ConnectionState.Connected))
                    .Merge(_centralManagerDelegate.DisconnectedPeripheralSubject.Where(x => Equals(x.Item2.Identifier, ((Device)device).Peripheral.Identifier))
                    .Select(x => ConnectionState.Disconnected));
        }

        public void Dispose()
        {
            
        }
    }
}