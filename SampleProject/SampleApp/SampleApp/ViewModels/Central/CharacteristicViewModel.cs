using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core;

namespace SampleApp.ViewModels.Central
{
    public class CharacteristicViewModel : BindableBase, INavigationAware
    {
        private ICharacteristic _characteristic;

        public CharacteristicViewModel()
        {
            ReadValueCommand = new DelegateCommand(ReadValue);
        }

        public ICharacteristic Characteristic
        {
            get { return _characteristic; }
            set
            {
                _characteristic = value; 
                Update();
            }
        }

        public Guid Uuid => _characteristic?.Uuid ?? Guid.Empty;
        public bool CanRead => _characteristic?.Properties.HasFlag(CharacteristicProperty.Read) ?? false;
        public bool CanWrite => _characteristic?.Properties.HasFlag(CharacteristicProperty.Write) ?? false;
        public bool CanNotify => (_characteristic?.Properties.HasFlag(CharacteristicProperty.Notify) ?? false)|| (_characteristic?.Properties.HasFlag(CharacteristicProperty.Indicate) ?? false);
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
        }

        private void Update()
        {
            OnPropertyChanged(() => Uuid);
            OnPropertyChanged(() => CanRead);
            OnPropertyChanged(() => CanWrite);
            OnPropertyChanged(() => CanNotify);
            OnPropertyChanged(() => Properties);
        }

        private void ReadValue()
        {
            
        }
    }
}
