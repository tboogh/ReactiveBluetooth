using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Extensions;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using SampleApp.Common.Behaviors;
using Xamarin.Forms;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace SampleApp.ViewModels.Peripheral
{
    public class PeripheralModeViewModel : BindableBase, INavigationAware, IPageAppearingAware, IDisposable
    {
        private readonly IPeripheralManager _peripheralManager;
        private string _state;
        private bool _advertising;
        private IDisposable _stateDisposable;
        private IDisposable _advertiseDisposable;
        private byte[] _writeValue = new byte[] {0xB0, 0x0B};
        private IDisposable _writeDisposable;
        private IDisposable _writeWithoutResponseDisposable;
        private IDisposable _readDisposable;
        private IDisposable _writeReadDisposable;
        private IDisposable _writeWithoutResponseReadDisposable;
        private IDisposable _unsubscribedDisposable;
        private IDisposable _subscribedDisposable;
        private readonly IList<IDevice> _subscribedDevices;
        private CancellationTokenSource _notifyLoopCancellationTokenSource;

        public PeripheralModeViewModel(IPeripheralManager peripheralManager)
        {
            _peripheralManager = peripheralManager;
            _subscribedDevices = new List<IDevice>();

            AdvertiseCommand = new DelegateCommand(StartAdvertise);
            StopAdvertiseCommand = new DelegateCommand(StopAdvertise);
        }

        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public bool Advertising
        {
            get { return _advertising; }
            set { SetProperty(ref _advertising, value); }
        }

        public string WriteValue => BitConverter.ToString(_writeValue);

        public DelegateCommand AdvertiseCommand { get; }
        public DelegateCommand StopAdvertiseCommand { get; }

        public void StartAdvertise()
        {
            if (_advertiseDisposable != null)
                return;

            var service = _peripheralManager.Factory.CreateService(Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"), ServiceType.Primary);
            var readCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"), new byte[] {0xB0, 0x06}, CharacteristicPermission.Read, CharacteristicProperty.Read);

            _readDisposable = readCharacterstic.ReadRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine("Read request");
                _peripheralManager.SendResponse(request, 0, new byte[] {0xB0, 0x0B});
            });

            var writeCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060002-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read | CharacteristicPermission.Write, CharacteristicProperty.Read | CharacteristicProperty.Write);

            _writeDisposable = writeCharacterstic.WriteRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine($"Write request. Value: {BitConverter.ToString(request.Value)}");
                SetWriteValue(request.Value);
                _peripheralManager.SendResponse(request, 0, _writeValue);
            });
            _writeReadDisposable = writeCharacterstic.ReadRequestObservable.Subscribe(request => { _peripheralManager.SendResponse(request, 0, _writeValue); });

            var writeWithoutResponseCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060003-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read | CharacteristicPermission.Write, CharacteristicProperty.Read | CharacteristicProperty.WriteWithoutResponse);

            _writeWithoutResponseDisposable = writeWithoutResponseCharacterstic.WriteRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine($"Write without response request. Value: {BitConverter.ToString(request.Value)}");
                SetWriteValue(request.Value);
            });

            _writeWithoutResponseReadDisposable = writeWithoutResponseCharacterstic.ReadRequestObservable.Subscribe(request => { _peripheralManager.SendResponse(request, 0, _writeValue); });

            var notifyCharacteristic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060004-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read, CharacteristicProperty.Notify);

            _subscribedDisposable = notifyCharacteristic.Subscribed.Subscribe(device =>
            {
                _subscribedDevices.Add(device);
            });

            _unsubscribedDisposable = notifyCharacteristic.Unsubscribed.Subscribe(device =>
            {
                _subscribedDevices.Remove(device);
            });

            if (!service.AddCharacteristic(writeCharacterstic))
            {
                throw new Exception("Failed to add write characteristic");
            }
            if (!service.AddCharacteristic(readCharacterstic))
            {
                throw new Exception("Failed to add read characteristic");
            }
            if (!service.AddCharacteristic(writeWithoutResponseCharacterstic))
            {
                throw new Exception("Failed to add write without response characteristic");
            }
            if (!service.AddCharacteristic(notifyCharacteristic))
            {
                throw new Exception("Failed to notify characteristic");
            }
            _advertiseDisposable = _peripheralManager.Advertise(new AdvertisingOptions() {LocalName = "TP", ServiceUuids = new List<Guid>() {Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62")}}, new List<IService> {service})
                .Subscribe(b => { Advertising = b; });

            _notifyLoopCancellationTokenSource?.Cancel();
            _notifyLoopCancellationTokenSource = new CancellationTokenSource();

            var t = Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    foreach (var subscribedDevice in _subscribedDevices)
                    {
                        if (!_peripheralManager.Notify(subscribedDevice, notifyCharacteristic, BitConverter.GetBytes(DateTime.Now.Second)))
                        {
                            // delay until write is ready
                        }
                    }
                }
                while (!_notifyLoopCancellationTokenSource.IsCancellationRequested);
                var xz = 0;
            }, _notifyLoopCancellationTokenSource.Token);
        }

        public void StopAdvertise()
        {
            _advertiseDisposable?.Dispose();
            _writeDisposable?.Dispose();
            _readDisposable?.Dispose();
            
            _writeReadDisposable?.Dispose();
            _stateDisposable?.Dispose();
            _writeWithoutResponseDisposable?.Dispose();
            _writeWithoutResponseReadDisposable?.Dispose();
            _subscribedDisposable?.Dispose();
            _unsubscribedDisposable?.Dispose();

            _subscribedDevices.Clear();

            _notifyLoopCancellationTokenSource?.Cancel();
            
            _advertiseDisposable = null;
            Advertising = false;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            StopAdvertise();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnAppearing(Page page)
        {
            _stateDisposable = _peripheralManager.State()
                .Subscribe(state => { State = state.ToString(); });
        }

        public void OnDisappearing(Page page)
        {
            StopAdvertise();
        }

        private void SetWriteValue(byte[] writeValue)
        {
            _writeValue = writeValue;
            OnPropertyChanged(() => WriteValue);
        }

        public void Dispose()
        {
            
        }
    }
}