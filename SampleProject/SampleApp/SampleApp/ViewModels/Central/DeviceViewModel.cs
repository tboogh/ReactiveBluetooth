using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Prism.Common;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using SampleApp.Common.Behaviors;
using SampleApp.Views;
using Xamarin.Forms;
using IService = ReactiveBluetooth.Core.Central.IService;

namespace SampleApp.ViewModels.Central
{
    public class DeviceViewModel : BindableBase, INavigationAware, IPageAppearingAware
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
        private DeviceViewModel _deviceViewModel;

        private CancellationTokenSource _cancellationTokenSource;

        public DeviceViewModel(ICentralManager centralManager)
        {
            _centralManager = centralManager;
            UpdateRssiCommand = new DelegateCommand(UpdateRssi);
            ItemSelectedCommand = DelegateCommand<CharacteristicViewModel>.FromAsyncHandler(CharacteristicSelected);
            Services = new ObservableCollection<Grouping<ServiceViewModel, CharacteristicViewModel>>();
            _cancellationTokenSource = new CancellationTokenSource();
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
                _connectionStateDisposable = _centralManager.ConnectToDevice(_deviceViewModel.Device)
                    .SubscribeOn(SynchronizationContext.Current)
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
            try
            {
                IList<IService> services = await _device.DiscoverServices(_cancellationTokenSource.Token);
                foreach (var service in services)
                {
                    ServiceViewModel serviceViewModel = new ServiceViewModel(service);
                    await serviceViewModel.DiscoverCharacteristics(_cancellationTokenSource.Token);

                    var grouping = new Grouping<ServiceViewModel, CharacteristicViewModel>(serviceViewModel,
                        serviceViewModel.Characteristics);
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { Services.Add(grouping); });
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task CharacteristicSelected(CharacteristicViewModel characteristicViewModel)
        {
            await
                _navigationService.NavigateAsync(nameof(CharacteristicDetailPage),
                    new NavigationParameters()
                    {
                        {nameof(ICharacteristic), characteristicViewModel.Characteristic},
                        {nameof(IDevice), Device }
                    
                    });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(nameof(DeviceViewModel)))
            {
                DeviceViewModel deviceViewModel = (DeviceViewModel) parameters[nameof(DeviceViewModel)];
                _deviceViewModel = deviceViewModel;
                Device = deviceViewModel.Device;

                Task.Factory.StartNew(Connect, CancellationToken.None, TaskCreationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void OnAppearing(Page page)
        {
            
        }

        public void OnDisappearing(Page page)
        {
            _cancellationTokenSource?.Cancel();
            _connectionStateDisposable?.Dispose();
        }
    }
}