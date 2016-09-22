using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using System.Reactive.Linq;
using System.Threading;

namespace SampleApp.ViewModels.Central
{
    public class CharacteristicViewModel : BindableBase, INavigationAware
    {
        private ICharacteristic _characteristic;
        private string _value;

        public CharacteristicViewModel()
        {
            ReadValueCommand = DelegateCommand.FromAsyncHandler(ReadValue);
        }

        public IDevice Device { get; set; }

        public ICharacteristic Characteristic
        {
            get { return _characteristic; }
            set
            {
                _characteristic = value;
                Update();
            }
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public Guid Uuid => _characteristic?.Uuid ?? Guid.Empty;
        public bool CanRead => _characteristic?.Properties.HasFlag(CharacteristicProperty.Read) ?? false;
        public bool CanWrite => _characteristic?.Properties.HasFlag(CharacteristicProperty.Write) ?? false;
        public bool CanNotify => (_characteristic?.Properties.HasFlag(CharacteristicProperty.Notify) ?? false) || (_characteristic?.Properties.HasFlag(CharacteristicProperty.Indicate) ?? false);
        public CharacteristicProperty Properties => _characteristic?.Properties ?? 0;
        public DelegateCommand ReadValueCommand { get; }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(nameof(ICharacteristic)))
            {
                ICharacteristic characteristic = (ICharacteristic) parameters[nameof(ICharacteristic)];
                Characteristic = characteristic;
            }
            if (parameters.ContainsKey(nameof(IDevice)))
            {
                IDevice device = (IDevice) parameters[nameof(IDevice)];
                Device = device;
            }
        }

        private void Update()
        {
            OnPropertyChanged(() => Uuid);
            OnPropertyChanged(() => CanRead);
            OnPropertyChanged(() => CanWrite);
            OnPropertyChanged(() => CanNotify);
            OnPropertyChanged(() => Properties);
        }

        private async Task ReadValue()
        {
            if (Device == null)
                return;

            var result = await Device.ReadValue(Characteristic, CancellationToken.None);
            Value = BitConverter.ToString(result);
        }
    }
}