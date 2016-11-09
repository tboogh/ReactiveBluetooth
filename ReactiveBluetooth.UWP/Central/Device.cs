using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace ReactiveBluetooth.UWP.Central
{
    public class Device : IDevice
    {
        private readonly BluetoothLEAdvertisement _advertisement;

        public Device(BluetoothLEAdvertisement advertisement, ulong bluetoothAddress)
        {
            byte[] uuidBytes = BitConverter.GetBytes(bluetoothAddress);
            Uuid = new Guid(uuidBytes);
            _advertisement = advertisement;
        }

        public string Name => _advertisement.LocalName;
        public Guid Uuid { get; }
        public ConnectionState State { get; }
        public IAdvertisementData AdvertisementData { get; }
        public Task<IList<IService>> DiscoverServices(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IObservable<int> Rssi { get; }
        public void UpdateRemoteRssi()
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadValue(ICharacteristic characteristic, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadValue(IDescriptor descriptor, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteValue(ICharacteristic characteristic, byte[] value, WriteType writeType, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteValue(IDescriptor descriptor, byte[] value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IObservable<byte[]> Notifications(ICharacteristic characteristic)
        {
            throw new NotImplementedException();
        }

        public bool RequestConnectionPriority(ConnectionPriority priority)
        {
            throw new NotImplementedException();
        }
    }
}
