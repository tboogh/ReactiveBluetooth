using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using CoreBluetooth;
using CoreFoundation;
using Foundation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Peripheral;
using ICharacteristic = ReactiveBluetooth.Core.Peripheral.ICharacteristic;
using IDevice = ReactiveBluetooth.Core.Peripheral.IDevice;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class PeripheralManager : IPeripheralManager
    {
        private readonly CBPeripheralManager _peripheralManager;
        private readonly PeripheralManagerDelegate.PeripheralManagerDelegate _peripheralDelegate;

        public PeripheralManager()
        {
            _peripheralDelegate = new PeripheralManagerDelegate.PeripheralManagerDelegate();
            _peripheralManager = new CBPeripheralManager(_peripheralDelegate, DispatchQueue.MainQueue);
            Factory = new AbstractFactory(_peripheralDelegate);
        }

        public IObservable<ManagerState> State()
        {
            return _peripheralDelegate.StateUpdatedSubject.Select(x => (ManagerState) x);
        }

        public IBluetoothAbstractFactory Factory { get; }

        public IObservable<bool> Advertise(AdvertisingOptions advertisingOptions, IList<IService> services)
        {
            var advertiseObservable = Observable.Create<bool>(observer =>
            {
               
                foreach (var service in services)
                {
                    var nativeService = (Service) service;
                    _peripheralManager.AddService(nativeService.MutableService);
                }

                StartAdvertisingOptions options = CreateAdvertisementOptions(advertisingOptions);
                _peripheralManager.StartAdvertising(options);
                return Disposable.Create(() =>
                {
                    _peripheralManager.StopAdvertising();
                    _peripheralManager.RemoveAllServices();
                });
            })
                .Publish()
                .RefCount();
            return advertiseObservable.Merge(_peripheralDelegate.AdvertisingStartedSubject);
        }

        public StartAdvertisingOptions CreateAdvertisementOptions(AdvertisingOptions advertisingOptions)
        {
            var options = new StartAdvertisingOptions();
            if (advertisingOptions.LocalName != null)
            {
                options.LocalName = advertisingOptions.LocalName;
            }

            if (advertisingOptions.ServiceUuids != null)
                options.ServicesUUID = advertisingOptions.ServiceUuids.Select(x => CBUUID.FromString(x.ToString()))
                    .ToArray();

            return options;
        }

        public IObservable<bool> AddService(IService service)
        {
            var nativeService = ((Service) service).MutableService;
            _peripheralManager.AddService(nativeService);
            return _peripheralDelegate.ServiceAddedSubject.Select(x => x.Service.UUID == nativeService.UUID);
        }

        public void RemoveService(IService service)
        {
            var nativeService = ((Service) service).MutableService;
            _peripheralManager.RemoveService(nativeService);
        }

        public void RemoveAllServices()
        {
            _peripheralManager.RemoveAllServices();
        }

        public bool SendResponse(IAttRequest request, int offset, byte[] value)
        {
            var attRequest = (AttRequest) request;
            var r = attRequest.CbAttRequest;
            r.Value = NSData.FromArray(value);

            _peripheralManager.RespondToRequest(r, CBATTError.Success);
            return true;
        }

        public bool Notify(IDevice device, ICharacteristic characteristic, byte[] value)
        {
            Device iosDevice = (Device) device;
            Characteristic iosCharacteristic = (Characteristic) characteristic;
            return _peripheralManager.UpdateValue(NSData.FromArray(value), iosCharacteristic.NativeCharacteristic, new[] {iosDevice.NativeCentral});
        }
    }
}