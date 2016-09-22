using System;
using System.Reactive.Linq;
using CoreBluetooth;

namespace Issue_14
{
    public class CentralManager
    {
        private readonly CBCentralManager _centralManager;
        private readonly CentralManagerDelegate _centralManagerDelegate;

        public CentralManager()
        {
            _centralManagerDelegate = new CentralManagerDelegate();
            _centralManager = new CBCentralManager(_centralManagerDelegate, null);
        }

        public IObservable<ManagerState> State()
        {
            return _centralManagerDelegate.StateUpdatedSubject.Select(x => (ManagerState)x);
        }

        public IObservable<Device> ScanForDevices()
        {
            var scanObservable = Observable.FromEvent<Device>(action => { _centralManager.ScanForPeripherals((CBUUID[])null); }, action => { _centralManager.StopScan(); });
            return scanObservable.Merge(_centralManagerDelegate.DiscoveredPeriperhalSubject.Select(x =>
            {
                Device device = new Device(x.Item2, x.Item4.Int32Value);
                return device;
            }));

        }

        public IObservable<ConnectionState> ConnectToDevice(Device device)
        {
            var nativeDevice = ((Device)device).Peripheral;

            return Observable.FromEvent<ConnectionState>(action =>
            {
                var d = nativeDevice;
                _centralManager.ConnectPeripheral(d);
                action(device.State);
            }, action =>
            {
                _centralManager.CancelPeripheralConnection(nativeDevice);
            }).Merge(_centralManagerDelegate.ConnectedPeripheralSubject
                    .Where(x => Equals(x.Item2.Identifier, nativeDevice.Identifier))
                    .Select(x => ConnectionState.Connected))
                    .Merge(_centralManagerDelegate.DisconnectedPeripheralSubject.Where(x => Equals(x.Item2.Identifier, nativeDevice.Identifier))
                    .Select(x => ConnectionState.Disconnected));
        }

        public void Dispose()
        {

        }
    }
}