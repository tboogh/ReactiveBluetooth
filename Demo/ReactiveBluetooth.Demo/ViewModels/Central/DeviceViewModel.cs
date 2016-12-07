using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.Common;
using Demo.Common.Behaviors;
using Demo.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;
using Xamarin.Forms;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace Demo.ViewModels.Central
{
    public class DeviceViewModel : BindableBase, INavigationAware, IPageAppearingAware, INavigationBackAware
    {
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
        private CancellationTokenSource _cancellationTokenSource;
        private IObservable<ConnectionState> _connectObservable;
        private IDisposable _deviceConnectionStateDisposable;

        public DeviceViewModel(ICentralManager centralManager)
        {
            _centralManager = centralManager;
            UpdateRssiCommand = new DelegateCommand(UpdateRssi);
            ItemSelectedCommand = DelegateCommand<CharacteristicViewModel>.FromAsyncHandler(CharacteristicSelected);
            SetConnectionParameterCommand = new DelegateCommand<ConnectionPriority?>(priority =>
            {
                var task = SetConnectionPriority(priority);
            });
            Services = new ObservableCollection<Grouping<ServiceViewModel, CharacteristicViewModel>>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public DeviceViewModel(ICentralManager centralManager, INavigationService navigationService, IPageDialogService pageDialogService) : this(centralManager)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
        }

        public DelegateCommand<CharacteristicViewModel> ItemSelectedCommand { get; }
        public DelegateCommand<ConnectionPriority?> SetConnectionParameterCommand { get; }

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

        void Disconnect()
        {
            _centralManager.Disconnect(Device, CancellationToken.None);
        }

        public async Task Connect()
        {
            Disconnect();
            _connectionStateDisposable?.Dispose();
            _connectObservable = _centralManager.Connect(Device)
                .SubscribeOn(SynchronizationContext.Current);
            try
            {
                _connectionStateDisposable = _connectObservable.Subscribe(async state =>
                {
                    if (state == ConnectionState.Connected)
                    {
                        await DiscoverServices();
                    }
                    if (state == ConnectionState.Disconnected)
                    {
                        _cancellationTokenSource.Cancel();
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
            try
            {
                IList<IService> services = await _device.DiscoverServices(_cancellationTokenSource.Token);
                await AddService(services);
            }
            catch (TaskCanceledException)
            {
            }
        }

        private async Task AddService(IList<IService> services)
        {
            foreach (var service in services)
            {
                ServiceViewModel serviceViewModel = new ServiceViewModel(service);
                await serviceViewModel.DiscoverCharacteristics(_cancellationTokenSource.Token);

                var grouping = new Grouping<ServiceViewModel, CharacteristicViewModel>(serviceViewModel, serviceViewModel.Characteristics);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Services.Add(grouping); });

                var includeServices = await service.DiscoverIncludedServices(_cancellationTokenSource.Token);
                if (includeServices != null && includeServices.Count > 0)
                {
                    await AddService(includeServices);
                }
            }
        }

        private async Task CharacteristicSelected(CharacteristicViewModel characteristicViewModel)
        {
            if (characteristicViewModel == null)
                return;
            if (ConnectionState != ConnectionState.Connected)
                return;

            await _navigationService.NavigateAsync(nameof(CharacteristicDetailPage), new NavigationParameters() {{nameof(ICharacteristic), characteristicViewModel.Characteristic}, {nameof(IDevice), Device}, {"ConnectionObservable", _connectObservable}});
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(nameof(IDevice)))
            {
                IDevice device = (IDevice) parameters[nameof(IDevice)];
                Device = device;
                _deviceConnectionStateDisposable = Device.ConnectionState.SubscribeOn(SynchronizationContext.Current)
                    .Subscribe(state => { ConnectionState = state; });

                var task = Connect();
            }
        }

        public void OnAppearing(Page page)
        {
        }

        public void OnDisappearing(Page page)
        {
        }

        public void PagePopped()
        {
            var disconnectTask = _centralManager.Disconnect(Device, CancellationToken.None);
            _cancellationTokenSource?.Cancel();
            _connectionStateDisposable?.Dispose();
            _deviceConnectionStateDisposable?.Dispose();
        }

        private async Task SetConnectionPriority(ConnectionPriority? connectionPriority)
        {
            if (connectionPriority == null)
                return;

            if (!_device.RequestConnectionPriority(connectionPriority.Value))
            {
                await _pageDialogService.DisplayAlertAsync("Error", $"Failed to set connection priority to {Enum.GetName(typeof(ConnectionPriority), connectionPriority.Value)}", "Ok");
            }
        }
    }
}