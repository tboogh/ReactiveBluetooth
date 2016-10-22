using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Demo.Common.Behaviors;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using Xamarin.Forms;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace Demo.ViewModels.Peripheral
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
        private IDisposable _notifyUnsubscribedDisposable;
        private IDisposable _notifySubscribedDisposable;
        private CancellationTokenSource _notifyLoopCancellationTokenSource;
        private IDisposable _indicateSubscribedDisposable;
        private IDisposable _indicateUnsubscribedDisposable;

		private readonly IList<IDevice> _notifySubscribedDevices;
		private readonly IList<IDevice> _indicateSubscribedDevices;

        public PeripheralModeViewModel(IPeripheralManager peripheralManager)
        {
            _peripheralManager = peripheralManager;
            _notifySubscribedDevices = new List<IDevice>();
			_indicateSubscribedDevices = new List<IDevice>();
			
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

            var service = _peripheralManager.Factory.CreateService(Guid.Parse("B0064000-0234-49D9-8439-39100D7EBD62"), ServiceType.Primary);
            var readCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0064001-0234-49D9-8439-39100D7EBD62"), new byte[] {0xB0, 0x06}, CharacteristicPermission.Read, CharacteristicProperty.Read);

            _readDisposable = readCharacterstic.ReadRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine("Read request");
                _peripheralManager.SendResponse(request, 0, new byte[] {0xB0, 0x0B});
            });

            var writeCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0064002-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read | CharacteristicPermission.Write, CharacteristicProperty.Read | CharacteristicProperty.Write);

            _writeDisposable = writeCharacterstic.WriteRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine($"Write request. Value: {BitConverter.ToString(request.Value)}");
                SetWriteValue(request.Value);
                _peripheralManager.SendResponse(request, 0, _writeValue);
            });
            _writeReadDisposable = writeCharacterstic.ReadRequestObservable.Subscribe(request => { _peripheralManager.SendResponse(request, 0, _writeValue); });

            var writeWithoutResponseCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0064003-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read | CharacteristicPermission.Write, CharacteristicProperty.Read | CharacteristicProperty.WriteWithoutResponse);

            _writeWithoutResponseDisposable = writeWithoutResponseCharacterstic.WriteRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine($"Write without response request. Value: {BitConverter.ToString(request.Value)}");
                SetWriteValue(request.Value);
            });

            _writeWithoutResponseReadDisposable = writeWithoutResponseCharacterstic.ReadRequestObservable.Subscribe(request => { _peripheralManager.SendResponse(request, 0, _writeValue); });

            var notifyCharacteristic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0064004-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read, CharacteristicProperty.Notify);

            _notifySubscribedDisposable = notifyCharacteristic.Subscribed.Subscribe(device =>
            {
                _notifySubscribedDevices.Add(device);
            });

            _notifyUnsubscribedDisposable = notifyCharacteristic.Unsubscribed.Subscribe(device =>
            {
                _notifySubscribedDevices.Remove(device);
            });

            var indicateCharacteristic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0064005-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read, CharacteristicProperty.Indicate);
            _indicateSubscribedDisposable = indicateCharacteristic.Subscribed.Subscribe(device =>
            {
                _indicateSubscribedDevices.Add(device);
            });

            _indicateUnsubscribedDisposable = indicateCharacteristic.Unsubscribed.Subscribe(device =>
            {
                _indicateSubscribedDevices.Remove(device);
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
            if (!service.AddCharacteristic(indicateCharacteristic))
            {
                throw new Exception("Failed to indicate characteristic");
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

                    foreach (var subscribedDevice in _notifySubscribedDevices)
                    {
                        if (!_peripheralManager.Notify(subscribedDevice, notifyCharacteristic, BitConverter.GetBytes(DateTime.Now.Second)))
                        {
                            // delay until write is ready
                        }
                    }
                    
                    foreach (var subscribedDevice in _indicateSubscribedDevices)
                    {
                        if (!_peripheralManager.Notify(subscribedDevice, notifyCharacteristic, BitConverter.GetBytes(60 - DateTime.Now.Second)))
                        {
                            // delay until write is ready
                        }
                    }
                }
                while (!_notifyLoopCancellationTokenSource.IsCancellationRequested);
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
            _notifySubscribedDisposable?.Dispose();
            _notifyUnsubscribedDisposable?.Dispose();
            _indicateSubscribedDisposable?.Dispose();
            _indicateUnsubscribedDisposable?.Dispose();

            _notifySubscribedDevices.Clear();

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