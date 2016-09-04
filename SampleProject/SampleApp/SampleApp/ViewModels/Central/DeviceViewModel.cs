using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using Reactive.Bindings;
using ReactiveBluetooth.Core.Central;

namespace SampleApp.ViewModels.Central
{
    public class DeviceViewModel : BindableBase, INavigationAware
    {
        private readonly ICentralManager _centralManager;

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
            ConnectCommand = new DelegateCommand(Connect);
            DisconnectCommand = DelegateCommand.FromAsyncHandler(Disconnect);
            UpdateRssiCommand = new DelegateCommand(UpdateRssi);
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

        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set { SetProperty(ref _connectionState, value); }
        }

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

        public void Connect()
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

        private async Task DiscoverServices()
        {
            var services = await _device.DiscoverServices();
        }

        public async Task Disconnect()
        {
            await _centralManager.DisconnectDevice(Device);
            _connectionStateDisposable.Dispose();
            ConnectionState = ConnectionState.Disconnected;
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