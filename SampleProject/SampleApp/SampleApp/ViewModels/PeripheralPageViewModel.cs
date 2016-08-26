using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Common;
using Prism.Navigation;
using WorkingNameBle.Core.Peripheral;
using Xamarin.Forms;

namespace SampleApp.ViewModels
{
    public class PeripheralPageViewModel : BindableBase, INavigationAware
    {
        private readonly IPeripheralManager _peripheralManager;
        private string _state;
        private bool _advertising;
        private IDisposable _stateDisposable;
        private IDisposable _advertiseDisposable;

        public PeripheralPageViewModel(IPeripheralManager peripheralManager)
        {
            _peripheralManager = peripheralManager;
            AdvertiseCommand = new DelegateCommand(StartAdvertise);
            StopAdvertiseCommand = new DelegateCommand(StopAdvertise);
            _stateDisposable = _peripheralManager.Init().Subscribe(state =>
            {
                State = state.ToString();
            });
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

            _advertiseDisposable = _peripheralManager.StartAdvertising(new AdvertisingOptions {ServiceUuids = new List<Guid>() { Guid.Parse("BC2F984A-0000-1000-8000-00805f9b34fb")} }).Subscribe(b =>
            { Advertising = b; });
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
