using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CoreBluetooth;

namespace Issue_14
{
    public class Device
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
    }
}