using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveBluetooth.Core.Central;

namespace SampleApp.ViewModels.Central
{
    public class DeviceViewModel : BindableBase
    {
        private readonly IDevice _device;
        private string _name;

        public DeviceViewModel(IDevice device)
        {
            _device = device;
            Name = _device.Name ?? "Unknown";
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
    }
}