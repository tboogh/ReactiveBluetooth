using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Java.Util;
using Plugin.CurrentActivity;
using ReactiveBluetooth.Android.Common;
using ReactiveBluetooth.Android.Extensions;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Exceptions;
using ReactiveBluetooth.Core.Types;
using Object = Java.Lang.Object;
using Observable = System.Reactive.Linq.Observable;

namespace ReactiveBluetooth.Android.Central
{
    public class CentralManager : ICentralManager
    {
        private readonly BluetoothAdapter _bluetoothAdapter;
        private IObservable<IDevice> _discoverObservable;
        private BroadcastListener _broadcastListener;

        public CentralManager()
        {
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            _broadcastListener = new BroadcastListener();
        }

        public void Dispose()
        {
            _broadcastListener.Dispose();
            _broadcastListener = null;
        }

        public IObservable<ManagerState> State()
        {
            return _broadcastListener.StateUpdatedSubject;
        }

        public IObservable<IDevice> ScanForDevices(IList<Guid> serviceUuids = null)
        {
            if (_discoverObservable == null)
            {
                // Store this and return the same someone else subscribes
                ScanCallback scanCallback = new ScanCallback();

                _discoverObservable = Observable.Create<IDevice>(observer =>
                    {
                        var scanFilters = serviceUuids?.Select(x =>
                            {
                                var scanFilterBuilder = new ScanFilter.Builder();
                                scanFilterBuilder.SetServiceUuid(new ParcelUuid(UUID.FromString(x.ToString())));
                                return scanFilterBuilder.Build();
                            })
                            .ToArray();
                        if (scanFilters != null)
                        {
                            var scanSettings = new ScanSettings.Builder().Build();

                            try
                            {
                                _bluetoothAdapter.BluetoothLeScanner.StartScan(scanFilters, scanSettings, scanCallback);
                            }
                            catch (Exception e)
                            {
                                observer.OnError(e);
                            }
                            
                        }
                        else
                        {
                            _bluetoothAdapter.BluetoothLeScanner.StartScan(scanCallback);
                        }
                        return Disposable.Create(() =>
                        {
                            _bluetoothAdapter.BluetoothLeScanner.StopScan(scanCallback);
                            _discoverObservable = null;
                        });
                    })
                    .Publish()
                    .RefCount()
                    .Merge(scanCallback.ScanResultSubject.Select(x =>
                    {
                        return new Device(x.Item2.Device, x.Item2.Rssi, new AdvertisementData(x.Item2.ScanRecord));
                        
                    }))
                    .Merge(scanCallback.FailureSubject.Select(failure =>
                    {
                        return default(Device);
                    }));
            }

            return _discoverObservable;
        }

        public IObservable<ConnectionState> Connect(IDevice device)
        {
            var androidDevice = (Device) device;
            var nativeDevice = androidDevice.NativeDevice;
            var context = Application.Context;
            var connectionObservable = Observable.Create<ConnectionState>(observer =>
            {
                var gatt = nativeDevice.ConnectGatt(context, false, androidDevice.GattCallback);
                androidDevice.Gatt = gatt;

                return Disposable.Empty;
            })
                .Merge(androidDevice.GattCallback.ConnectionStateChange.Select(x => (ConnectionState)x))
                .Publish()
                .RefCount();
            return connectionObservable;
        }

        public async Task Disconnect(IDevice device, CancellationToken cancellationToken)
        {
            var androidDevice = (Device)device;

            if (androidDevice.Gatt == null)
                return;

            androidDevice.Gatt?.Disconnect();
            try
            {
                await androidDevice.GattCallback.ConnectionStateChange.FirstAsync(x => x == ProfileState.Disconnected)
                    .ToTask(cancellationToken);
            }
            finally
            {
                androidDevice.Gatt?.Close();
                androidDevice.Gatt = null;
            }
        }
    }
}