using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;

namespace ReactiveBluetooth.UWP.Central
{
    public class CentralManager : ICentralManager
    {
        private BluetoothLEAdvertisementWatcher _advertismentWatcher;

        public CentralManager()
        {
            _advertismentWatcher = new BluetoothLEAdvertisementWatcher() {ScanningMode = BluetoothLEScanningMode.Active};
        }

        public IObservable<ManagerState> State()
        {
            return Observable.FromEventPattern<TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementWatcherStoppedEventArgs>, BluetoothLEAdvertisementWatcherStoppedEventArgs>(eh => _advertismentWatcher.Stopped += eh, eh => _advertismentWatcher.Stopped -= eh)
                .Select(x =>
                {
                    switch (x.EventArgs.Error)
                    {
                        case BluetoothError.Success:
                            return ManagerState.PoweredOn;
                        case BluetoothError.RadioNotAvailable:
                            return ManagerState.PoweredOff;
                        case BluetoothError.ResourceInUse:
                            return ManagerState.Unknown;
                        case BluetoothError.DeviceNotConnected:
                            return ManagerState.Unknown;
                        case BluetoothError.OtherError:
                            return ManagerState.Unsupported;
                        case BluetoothError.DisabledByPolicy:
                            return ManagerState.Unauthorized;
                        case BluetoothError.NotSupported:
                            return ManagerState.Unsupported;
                        case BluetoothError.DisabledByUser:
                            return ManagerState.Unauthorized;
                        case BluetoothError.ConsentRequired:
                            return ManagerState.Unauthorized;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                })
                .StartWith(ManagerState.Unknown);
        }

        public IObservable<IDevice> ScanForDevices(IList<Guid> serviceGuids = null)
        {
            var observable = Observable.FromEventPattern<TypedEventHandler<BluetoothLEAdvertisementWatcher, BluetoothLEAdvertisementReceivedEventArgs>, BluetoothLEAdvertisementReceivedEventArgs>(eh => _advertismentWatcher.Received += eh, eh => _advertismentWatcher.Received -= eh)
                .Select((pattern, i) => { return new Device(pattern.EventArgs.Advertisement, pattern.EventArgs.BluetoothAddress); })
                .Cast<IDevice>();

            var startObservable = Observable.Create<IDevice>(observer =>
            {
                _advertismentWatcher.Start();
                return Disposable.Create(() => { _advertismentWatcher.Stop(); });
            });

            return observable.Merge(startObservable);
        }

        public IObservable<ConnectionState> Connect(IDevice device)
        {
            throw new NotImplementedException();
        }

        public void Disconnect(IDevice device)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}