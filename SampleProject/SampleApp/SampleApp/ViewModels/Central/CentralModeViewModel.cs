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
using ReactiveBluetooth.Core.Central;
using SampleApp.ViewModels.Central;
using Xamarin.Forms;

namespace SampleApp.ViewModels
{
    public class CentralModeViewModel : BindableBase, INavigationAware
    {
        private readonly ICentralManager _centralManager;
        private string _state;

        private IDisposable _toggleScanDisposable;

        public CentralModeViewModel(ICentralManager centralManager, INavigationService navigationService)
        {
            _centralManager = centralManager;
            _centralManager.Init()
                .Subscribe(state =>
                { State = state.ToString(); });
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
                            Devices.Add(new DeviceViewModel(device));
                        }
                        else
                        {
                            currentDevice.UpdateDevice(device);
                        }
                    });
            }
        }

        public void ItemSelected(DeviceViewModel device)
        {
            Task.Run(async () =>
            {
                
            });
        }


        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            _toggleScanDisposable.Dispose();
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }
    }
}