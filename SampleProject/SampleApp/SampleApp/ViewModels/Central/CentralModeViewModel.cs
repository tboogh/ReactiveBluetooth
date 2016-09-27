using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prism.Common;
using Prism.Navigation;
using Prism.Services;
using ReactiveBluetooth.Core.Central;
using SampleApp.Common.Behaviors;
using SampleApp.ViewModels.Central;
using SampleApp.Views;
using Xamarin.Forms;

namespace SampleApp.ViewModels
{
    public class CentralModeViewModel : BindableBase, INavigationAware, IPageAppearingAware
    {
        private readonly ICentralManager _centralManager;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;
        private string _state;
        private IDisposable _toggleScanDisposable;

        public CentralModeViewModel(ICentralManager centralManager, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _centralManager = centralManager;
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            _centralManager.State()
                .Subscribe(state => { State = state.ToString(); });
            ToggleScanCommand = new DelegateCommand(ToggleScan);
            DeviceSelectedCommand = new DelegateCommand<DeviceViewModel>(ItemSelected);
            Devices = new ObservableCollection<DeviceViewModel>();
        }

        public DelegateCommand<DeviceViewModel> DeviceSelectedCommand { get; }
        public DelegateCommand ToggleScanCommand { get; }
        public ObservableCollection<DeviceViewModel> Devices { get; }

        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public void ToggleScan()
        {
            if (_toggleScanDisposable != null)
            {
                _toggleScanDisposable.Dispose();
                _toggleScanDisposable = null;
            }
            else
            {
                Devices.Clear();
                _toggleScanDisposable = _centralManager.ScanForDevices()
                    .Subscribe(device =>
                    {
                        var currentDevice = Devices.FirstOrDefault(x => x.Uuid == device.Uuid);
                        if (currentDevice == null)
                        {
                            currentDevice = new DeviceViewModel(_centralManager);
                            Devices.Add(currentDevice);
                        }
                        currentDevice.Device = device;
                    });
            }
        }

        public async void ItemSelected(DeviceViewModel device)
        {
            if (device == null)
                return;
            
            _toggleScanDisposable?.Dispose();

            await _pageDialogService.DisplayActionSheetAsync(device.Name, ActionSheetButton.CreateButton("Display Advertisedata", DelegateCommand.FromAsyncHandler(async () =>
            {
                await _navigationService.NavigateAsync(nameof(AdvertiseDataPage), new NavigationParameters() {
                    {
                        nameof(IAdvertisementData), device.Device.AdvertisementData

                    }
                });
            })), ActionSheetButton.CreateButton("Connect", DelegateCommand.FromAsyncHandler(async () =>
            {
                await _navigationService.NavigateAsync(nameof(DeviceDetailPage), new NavigationParameters() {
                    {
                        nameof(DeviceViewModel), device 
                    
                    }
                });
            })), ActionSheetButton.CreateCancelButton("Cancel", new DelegateCommand(() => {})));
            
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            _toggleScanDisposable?.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }

        public void OnAppearing(Page page)
        {
            
        }

        public void OnDisappearing(Page page)
        {
            _toggleScanDisposable?.Dispose();
        }
    }
}