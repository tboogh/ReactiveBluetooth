using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Peripheral;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace SampleApp.ViewModels.Peripheral
{
    public class PeripheralModeViewModel : BindableBase, INavigationAware
    {
        private readonly IPeripheralManager _peripheralManager;
        private string _state;
        private bool _advertising;
        private IDisposable _stateDisposable;
        private IDisposable _advertiseDisposable;
        private byte[] _writeValue = new byte[] {0xB0, 0x0B};

        public PeripheralModeViewModel(IPeripheralManager peripheralManager)
        {
            _peripheralManager = peripheralManager;
            AdvertiseCommand = new DelegateCommand(StartAdvertise);
            StopAdvertiseCommand = new DelegateCommand(StopAdvertise);
            _stateDisposable = _peripheralManager.State()
                .Subscribe(state => { State = state.ToString(); });
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

        public DelegateCommand AdvertiseCommand { get; }
        public DelegateCommand StopAdvertiseCommand { get; }

        public void StartAdvertise()
        {
            if (_advertiseDisposable != null)
                return;

            var service = _peripheralManager.Factory.CreateService(Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"), ServiceType.Primary);
            var readCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"), new byte[] {0xB0, 0x06}, CharacteristicPermission.Read, CharacteristicProperty.Read);

            readCharacterstic.ReadRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine("Read request");
                _peripheralManager.SendResponse(request, 0, new byte[] {0xB0, 0x0B});
            });

            var writeCharacterstic = _peripheralManager.Factory.CreateCharacteristic(Guid.Parse("B0060002-0234-49D9-8439-39100D7EBD62"), null, CharacteristicPermission.Read | CharacteristicPermission.Write, CharacteristicProperty.Read | CharacteristicProperty.Write);

            writeCharacterstic.WriteRequestObservable.Subscribe(request =>
            {
                Debug.WriteLine($"Write request. Value: {BitConverter.ToString(request.Value)}");
                _writeValue = request.Value;
                _peripheralManager.SendResponse(request, 0, _writeValue);
            });
            writeCharacterstic.ReadRequestObservable.Subscribe(request =>
            {
                _peripheralManager.SendResponse(request, 0, _writeValue);
            });

            if (!service.AddCharacteristic(writeCharacterstic))
            {
                throw new Exception("Failed to add write characteristic");
            }
            if (!service.AddCharacteristic(readCharacterstic))
            {
                throw new Exception("Failed to add read characteristic");
            }

            _advertiseDisposable = _peripheralManager.Advertise(new AdvertisingOptions() {LocalName = "TP", ServiceUuids = new List<Guid>() {Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62") }}, new List<IService> {service})
                .Subscribe(b => { Advertising = b; });
        }

        public void StopAdvertise()
        {
            _advertiseDisposable?.Dispose();
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
    }
}