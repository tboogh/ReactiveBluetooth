using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveBluetooth.Core.Central;
using SampleApp.ViewModels.Central;

namespace SampleApp.ViewModels
{
    public class CentralModeViewModel : BindableBase
    {
        private readonly ICentralManager _centralManager;
        private IDisposable _toggleScanDisposable;
        private string _state;

        public CentralModeViewModel(ICentralManager centralManager)
        {
            _centralManager = centralManager;
            _centralManager.Init()
                .Subscribe(state =>
                { State = state.ToString(); });
            ToggleScanCommand = new DelegateCommand(ToggleScan);
            Devices = new ObservableCollection<DeviceViewModel>();
        }

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
                _toggleScanDisposable = _centralManager.ScanForDevices()
                    .Distinct()
                    .Subscribe(device =>
                    {
                        Devices.Add(new DeviceViewModel(device));
                    });
            }
        }
    }
}