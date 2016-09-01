using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using WorkingNameBle.Core.Central;

namespace SampleApp.ViewModels
{
    public class CentralPageViewModel : BindableBase
    {
        private readonly ICentralManager _centralManager;
        private IDisposable _toggleScanDisposable;

        public CentralPageViewModel(ICentralManager centralManager)
        {
            _centralManager = centralManager;
            ToggleScanCommand = new DelegateCommand(ToggleScan);
            Devices = new ObservableCollection<IDevice>();
        }

        public DelegateCommand ToggleScanCommand { get; }
        public ObservableCollection<IDevice> Devices { get; }

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
                    .Subscribe(device => { });
            }
        }
    }
}