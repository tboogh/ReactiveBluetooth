using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Bindings;
using ReactiveBluetooth.Core.Central;

namespace SampleApp.ViewModels.Central
{
    public class DeviceViewModel : BindableBase
    {
        private IDevice _device;
        private string _name;
        private Guid _uuid;
        private int _rssi;
        private IDisposable _rssiDisposable;

        public DeviceViewModel(IDevice device)
        {
            UpdateDevice(device);
            UpdateRssiCommand = new DelegateCommand(_device.UpdateRemoteRssi);
        }

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

        public DelegateCommand UpdateRssiCommand { get; }

        public void UpdateDevice(IDevice device)
        {
            _device = device;

            Uuid = _device.Uuid;
            Name = _device.Name ?? "Unknown";
            _rssiDisposable?.Dispose();
            _rssiDisposable = _device.Rssi.Subscribe(rssi => { Rssi = rssi; });
        }
    }
}