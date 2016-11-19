using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.Common;
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

        private readonly byte[] _readValue = { 0xB0, 0x06, 0x00, 0x01 };
        private byte[] _writeValue = {0xB0, 0x06, 0x00, 0x02};
        private byte[] _writeWithoutResponseValue = { 0xB0, 0x06, 0x00, 0x03 };
        private byte[] _timeValue;
        private byte[] _reverseTimeValue;

        private string _state;
        private bool _advertising;
        private IDisposable _stateDisposable;
        private IDisposable _advertiseDisposable;
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
        private IDisposable _notifyReadDisposable;
        private readonly IList<IDevice> _notifySubscribedDevices;
        private readonly IList<IDevice> _indicateSubscribedDevices;

        public PeripheralModeViewModel(IPeripheralManager peripheralManager)
        {
            _peripheralManager = peripheralManager;
            _notifySubscribedDevices = new List<IDevice>();
            _indicateSubscribedDevices = new List<IDevice>();

            AdvertiseCommand = new DelegateCommand(StartAdvertise);
            StopAdvertiseCommand = new DelegateCommand(StopAdvertise);

            Services = new ObservableCollection<Grouping<ServiceViewModel, CharacteristicViewModel>>();
        }

        public ObservableCollection<Grouping<ServiceViewModel, CharacteristicViewModel>> Services { get; set; }

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
        
        public DelegateCommand AdvertiseCommand { get; }
        public DelegateCommand StopAdvertiseCommand { get; }

        public void StartAdvertise()
        {
            if (_advertiseDisposable != null)
                return;

            var service = _peripheralManager.Factory.CreateService(Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"), ServiceType.Primary);
            var readCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"), _readValue, CharacteristicPermission.Read, CharacteristicProperty.Read);
            var writeCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060002-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read | CharacteristicPermission.Write, CharacteristicProperty.Read | CharacteristicProperty.Write);
            var writeWithoutResponseCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060003-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read | CharacteristicPermission.Write, CharacteristicProperty.Read | CharacteristicProperty.WriteWithoutResponse);
            var notifyCharacteristic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060004-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read, CharacteristicProperty.Notify | CharacteristicProperty.Read);
            var indicateCharacteristic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060005-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read, CharacteristicProperty.Indicate | CharacteristicProperty.Read);

            var includeService = _peripheralManager.Factory.CreateService(Guid.Parse("B0120000-0234-49D9-8439-39100D7EBD62"), ServiceType.Secondary);
            if (!service.AddIncludeService(includeService))
            {
                throw new Exception("Failed to add include service");
            }

            ServiceViewModel serviceViewModel = new ServiceViewModel()
            {
                Uuid = service.Uuid
            };

            CharacteristicViewModel readCharacteristicViewModel = new CharacteristicViewModel { Uuid = readCharacterstic.Uuid, Properties = readCharacterstic.Properties, Value = BitConverter.ToString(readCharacterstic.Value) };
            CharacteristicViewModel writeCharacteristicViewModel = new CharacteristicViewModel { Uuid = writeCharacterstic.Uuid, Properties = writeCharacterstic.Properties, Value = BitConverter.ToString(_writeValue)};
            CharacteristicViewModel writeWithoutResponseCharacteristicViewModel = new CharacteristicViewModel { Uuid = writeWithoutResponseCharacterstic.Uuid, Properties = writeWithoutResponseCharacterstic.Properties, Value = BitConverter.ToString(_writeWithoutResponseValue)};
            CharacteristicViewModel notifyCharacteristicViewModel = new CharacteristicViewModel { Uuid = notifyCharacteristic.Uuid, Properties = notifyCharacteristic.Properties };
            CharacteristicViewModel indicateCharacteristicViewModel = new CharacteristicViewModel { Uuid = indicateCharacteristic.Uuid, Properties = indicateCharacteristic.Properties };

            _readDisposable = readCharacterstic.ReadRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine("Read request");
                _peripheralManager.SendResponse(request, 0, readCharacterstic.Value);
            });

            _writeDisposable = writeCharacterstic.WriteRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine($"Write request. Value: {BitConverter.ToString(request.Value)}");
                _writeValue = request.Value;
                writeCharacteristicViewModel.Value = BitConverter.ToString(_writeValue);
                _peripheralManager.SendResponse(request, 0, _writeValue);
            });
            _writeReadDisposable = writeCharacterstic.ReadRequestObservable.Subscribe(request => { _peripheralManager.SendResponse(request, 0, _writeValue); });

            _writeWithoutResponseDisposable = writeWithoutResponseCharacterstic.WriteRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine($"Write without response request. Value: {BitConverter.ToString(request.Value)}");
                _writeWithoutResponseValue = request.Value;
                writeWithoutResponseCharacteristicViewModel.Value = BitConverter.ToString(_writeWithoutResponseValue);
                _peripheralManager.SendResponse(request, 0, _writeWithoutResponseValue);
            });

            _writeWithoutResponseReadDisposable = writeWithoutResponseCharacterstic.ReadRequestObservable.Subscribe(request =>
            {
                _peripheralManager.SendResponse(request, 0, _writeWithoutResponseValue);
            });

            _notifyReadDisposable = notifyCharacteristic.ReadRequestObservable.Subscribe(request =>
            {
                _peripheralManager.SendResponse(request, 0, _timeValue);
            });

            _notifySubscribedDisposable = notifyCharacteristic.Subscribed.Subscribe(device =>
            {
                _notifySubscribedDevices.Add(device);
            });

            _notifyUnsubscribedDisposable = notifyCharacteristic.Unsubscribed.Subscribe(device =>
            {
                var d = _indicateSubscribedDevices.FirstOrDefault(x => x.Uuid == device.Uuid);
                _notifySubscribedDevices.Remove(d); 
            });

            _indicateSubscribedDisposable = indicateCharacteristic.Subscribed.Subscribe(device =>
            {
                _indicateSubscribedDevices.Add(device); 
            });

            _indicateUnsubscribedDisposable = indicateCharacteristic.Unsubscribed.Subscribe(device =>
            {
                var d = _indicateSubscribedDevices.FirstOrDefault(x => x.Uuid == device.Uuid);
                _indicateSubscribedDevices.Remove(d); 
            });

            indicateCharacteristic.ReadRequestObservable.Subscribe(request =>
            {
                _peripheralManager.SendResponse(request, 0, _reverseTimeValue);
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

            _advertiseDisposable = _peripheralManager.Advertise(new AdvertisingOptions()
            {
                LocalName = "TP",
                ServiceUuids = new List<Guid>() {service.Uuid},
                AdvertiseTx = AdvertiseTx.PowerHigh,
                AdvertiseMode = AdvertiseMode.LowLatency
            }, new List<IService> {service})
                .Subscribe(b => { Advertising = b; });

            _notifyLoopCancellationTokenSource?.Cancel();
            _notifyLoopCancellationTokenSource = new CancellationTokenSource();

            var t = Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    _timeValue = BitConverter.GetBytes(DateTime.Now.Second);
                    _reverseTimeValue = BitConverter.GetBytes(60 - DateTime.Now.Second);
                    notifyCharacteristic.Value = _timeValue;
                    indicateCharacteristic.Value = _timeValue;

                    notifyCharacteristicViewModel.Value = BitConverter.ToString(_timeValue);
                    indicateCharacteristicViewModel.Value = BitConverter.ToString(_reverseTimeValue);

                    foreach (var subscribedDevice in _notifySubscribedDevices)
                    {
                        if (!_peripheralManager.Notify(subscribedDevice, notifyCharacteristic, _timeValue))
                        {
                            // delay until write is ready
                        }
                    }

                    foreach (var subscribedDevice in _indicateSubscribedDevices)
                    {
                        if (!_peripheralManager.Notify(subscribedDevice, indicateCharacteristic, _reverseTimeValue))
                        {
                            // delay until write is ready
                        }
                    }
                }
                while (!_notifyLoopCancellationTokenSource.IsCancellationRequested);
            }, _notifyLoopCancellationTokenSource.Token);

            Grouping<ServiceViewModel, CharacteristicViewModel> grouping = new Grouping<ServiceViewModel, CharacteristicViewModel>(serviceViewModel,
                        new List<CharacteristicViewModel>()
                        {
                            readCharacteristicViewModel,
                            writeCharacteristicViewModel,
                            writeWithoutResponseCharacteristicViewModel,
                            notifyCharacteristicViewModel,
                            indicateCharacteristicViewModel
                        });

            Device.BeginInvokeOnMainThread(() =>
            {
                Services.Clear();
                Services.Add(grouping);
            });
            
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
            _notifyReadDisposable?.Dispose();
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

        public void Dispose()
        {
        }
    }
}