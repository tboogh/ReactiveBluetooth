using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using SampleApp.Common.Behaviors;
using Xamarin.Forms;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace SampleApp.ViewModels.Peripheral
{
    public class PeripheralModeViewModel : BindableBase, INavigationAware, IPageAppearingAware
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

        public PeripheralModeViewModel(IPeripheralManager peripheralManager)
        {
            _peripheralManager = peripheralManager;
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

            if (!service.AddCharacteristic(writeCharacterstic))
            {
                throw new Exception("Failed to add write characteristic");
            }
            if (!service.AddCharacteristic(readCharacterstic))
            {
                throw new Exception("Failed to add read characteristic");
            }

            _advertiseDisposable = _peripheralManager.Advertise(new AdvertisingOptions() {LocalName = "TP", ServiceUuids = new List<Guid>() {Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62")}}, new List<IService> {service})
                .Subscribe(b => { Advertising = b; });
        }

        public void StopAdvertise()
        {
            _advertiseDisposable?.Dispose();
            _writeDisposable?.Dispose();
            _readDisposable?.Dispose();
            ;
            _writeReadDisposable?.Dispose();
            _stateDisposable?.Dispose();
            _writeWithoutResponseDisposable?.Dispose();
            _writeWithoutResponseReadDisposable?.Dispose();
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
    }
}