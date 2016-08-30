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
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Peripheral;
using IService = WorkingNameBle.Core.Peripheral.IService;

namespace WorkingNameBle.iOS.Peripheral
{
    public class PeripheralManager : IPeripheralManager
    {
        private CBPeripheralManager _peripheralManager;
        private PeripheralManagerDelegate.PeripheralManagerDelegate _peripheralDelegate;

        public PeripheralManager()
        {
            Factory = new AbstractFactory(_peripheralDelegate);
        }

        public ManagerState State
        {
            get
            {
                if (_peripheralManager != null)
                    return (ManagerState) _peripheralManager.State;
                return ManagerState.PoweredOff;
            }
        }

        public IBluetoothAbstractFactory Factory { get; }

        public IObservable<ManagerState> Init(IScheduler scheduler = null)
        {
            _peripheralDelegate = new PeripheralManagerDelegate.PeripheralManagerDelegate();
            _peripheralManager = new CBPeripheralManager(_peripheralDelegate, DispatchQueue.MainQueue);

            return _peripheralDelegate.StateUpdatedSubject.Select(x => x != null ? (ManagerState)x.State : ManagerState.Unknown);
        }

        public void Shutdown()
        {
            _peripheralManager.StopAdvertising();
        }

        public IObservable<bool> StartAdvertising(AdvertisingOptions advertisingOptions, IList<IService> services)
        {

            var advertiseObservable = Observable.Create<bool>(observer =>
            {
                IDisposable started = _peripheralDelegate.AdvertisingStartedSubject.Subscribe(o =>
                {
                    observer.OnNext(o);
                });

                StartAdvertisingOptions options = CreateAdvertisementOptions(advertisingOptions);
                _peripheralManager.StartAdvertising(options);
                return Disposable.Create(() =>
                {
                    _peripheralManager.StopAdvertising();
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
    }
}
