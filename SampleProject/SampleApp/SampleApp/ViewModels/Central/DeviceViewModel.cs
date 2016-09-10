using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using SampleApp.Views;
using Xamarin.Forms;

namespace SampleApp.ViewModels.Central
{
    public class DeviceViewModel : BindableBase, INavigationAware
    {
        public class Grouping<K, T> : ObservableCollection<T>
        {
            public K Key { get; private set; }

            public Grouping(K key, IEnumerable<T> items)
            {
                Key = key;
                foreach (var item in items)
                    this.Items.Add(item);
            }
        }

        private readonly ICentralManager _centralManager;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;

        public IDevice Device
        {
            get { return _device; }
            set
            {
                _device = value;
                UpdateDevice(_device);
            }
        }

        private string _name;
        private Guid _uuid;
        private int _rssi;
        private IDisposable _rssiDisposable;
        private IDevice _device;
        private IDisposable _connectionStateDisposable;
        private ConnectionState _connectionState;

        public DeviceViewModel(ICentralManager centralManager)
        {
            _centralManager = centralManager;
            ConnectCommand = DelegateCommand.FromAsyncHandler(Connect);
            DisconnectCommand = DelegateCommand.FromAsyncHandler(Disconnect);
            UpdateRssiCommand = new DelegateCommand(UpdateRssi);
            ItemSelectedCommand = DelegateCommand<CharacteristicViewModel>.FromAsyncHandler(CharacteristicSelected);
            Services = new ObservableCollection<Grouping<ServiceViewModel, CharacteristicViewModel>>();
        }

        public DeviceViewModel(ICentralManager centralManager, INavigationService navigationService,
            IPageDialogService pageDialogService) : this(centralManager)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
        }

        public DelegateCommand<CharacteristicViewModel> ItemSelectedCommand { get; }

        public Guid Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public int Rssi
        {
            get { return _rssi; }
            set { SetProperty(ref _rssi, value); }
        }

        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set { SetProperty(ref _connectionState, value); }
        }

        public ObservableCollection<Grouping<ServiceViewModel, CharacteristicViewModel>> Services { get; set; }
        public DelegateCommand UpdateRssiCommand { get; }
        public DelegateCommand ConnectCommand { get; }
        public DelegateCommand DisconnectCommand { get; }

        public void UpdateDevice(IDevice device)
        {
            Uuid = Device.Uuid;
            Name = Device.Name ?? "Unknown";
            _rssiDisposable?.Dispose();
            _rssiDisposable = Device.Rssi.Subscribe(rssi => { Rssi = rssi; });
        }

        public void UpdateRssi()
        {
            Device?.UpdateRemoteRssi();
        }

        public async Task Connect()
        {
            _connectionStateDisposable?.Dispose();
            try
            {
                _connectionStateDisposable = _centralManager.ConnectToDevice(Device)
                    .Subscribe(async state =>
                    {
                        ConnectionState = state;
                        if (state == ConnectionState.Connected)
                        {
                            await DiscoverServices();
                        }
                    });
            }
            catch (TimeoutException exception)
            {
                await _pageDialogService.DisplayAlertAsync("Failed to connect", "Could not connect to device", "Ok");
            }
        }

        private async Task DiscoverServices()
        {
            Services.Clear();
            var services = await _device.DiscoverServices()
                .ToTask();
            foreach (var service in services)
            {
                ServiceViewModel serviceViewModel = new ServiceViewModel(service);
                await serviceViewModel.DiscoverCharacteristics();

                var grouping = new Grouping<ServiceViewModel, CharacteristicViewModel>(serviceViewModel,
                    serviceViewModel.Characteristics);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Services.Add(grouping); });
            }
        }

        public async Task Disconnect()
        {
            await _centralManager.DisconnectDevice(Device);
            _connectionStateDisposable.Dispose();
            ConnectionState = ConnectionState.Disconnected;
        }

        private async Task CharacteristicSelected(CharacteristicViewModel characteristicViewModel)
        {
            await
                _navigationService.NavigateAsync(nameof(CharacteristicDetailPage),
                    new NavigationParameters() {{nameof(ICharacteristic), characteristicViewModel.Characteristic}});
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("Device"))
            {
                IDevice device = (IDevice) parameters["Device"];
                Device = device;
            }
        }
    }
}