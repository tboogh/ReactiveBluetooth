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
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.iOS.Peripheral
{
    public class PeripheralManager : IPeripheralManager
    {
        private CBPeripheralManager _peripheralManager;
        private readonly PeripheralManagerDelegate.PeripheralManagerDelegate _peripheralDelegate;

        public PeripheralManager()
        {
            _peripheralDelegate = new PeripheralManagerDelegate.PeripheralManagerDelegate();
            _peripheralManager = new CBPeripheralManager(_peripheralDelegate, DispatchQueue.MainQueue);
            Factory = new AbstractFactory(_peripheralDelegate);
        }

        public IObservable<ManagerState> State()
        {
            return _peripheralDelegate.StateUpdatedSubject.Select(x => (ManagerState)x);
        }

        public IBluetoothAbstractFactory Factory { get; }

        public void Shutdown()
        {
            _peripheralManager.StopAdvertising();
        }

        public IObservable<bool> Advertise(AdvertisingOptions advertisingOptions, IList<IService> services)
        {

            var advertiseObservable = Observable.Create<bool>(observer =>
            {
                IDisposable started = _peripheralDelegate.AdvertisingStartedSubject.Subscribe(o =>
                {
                    observer.OnNext(o);
                });

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
                    started.Dispose();
                });
            });
            return advertiseObservable;
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
            return _peripheralDelegate.ServiceAddedSubject.Select(x =>
            {
                return x.Service.UUID == nativeService.UUID;
            })
                .Catch(Observable.Return(false));
        }

        public void RemoveService(IService service)
        {
            var nativeService = ((Service)service).MutableService;
            _peripheralManager.RemoveService(nativeService);
        }

        public void RemoveAllServices()
        {
            _peripheralManager.RemoveAllServices();
        }

        public bool SendResponse(IAttRequest request, int offset, byte[] value)
        {
            var attRequest = (AttRequest) request;
            var r = attRequest.CBAttRequest;
            r.Value = NSData.FromArray(value);
            
            _peripheralManager.RespondToRequest(r, CBATTError.Success);
            return true;
        }
    }
}
