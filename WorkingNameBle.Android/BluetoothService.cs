using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Exceptions;

namespace WorkingNameBle.Android
{
    public class BluetoothService : IBluetoothService
    {
        private BluetoothAdapter _bluetoothAdapter;
        private bool _initialized;
        private IObservable<IDevice> _discoverObservable;

        public void Init(IScheduler scheduler = null)
        {
            if (_initialized)
            {
                _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            }
            _initialized = true;
        }

        public void Shutdown()
        {
            _initialized = false;
        }

        public Task<bool> ReadyToDiscover()
        {
            return Task.FromResult(true);
        }

        public IObservable<IDevice> ScanForDevices()
        {
            if (_discoverObservable == null)
            {
                // Store this and return the same someone else subscribes
                _discoverObservable = Observable.Create<IDevice>(observer =>
                {
                    BleScanCallback bleScanCallback = new BleScanCallback((type, result) =>
                    {
                        var device = new Device(result.Device);
                        observer.OnNext(device);
                    }, failure =>
                    {
                        observer.OnError(new DiscoverDeviceException(failure.ToString()));
                    });

                    _bluetoothAdapter.BluetoothLeScanner.StartScan(bleScanCallback);

                    return Disposable.Create(() => { _bluetoothAdapter.BluetoothLeScanner.StopScan(bleScanCallback); });
                });
            }
            return _discoverObservable;
        }
    }
}