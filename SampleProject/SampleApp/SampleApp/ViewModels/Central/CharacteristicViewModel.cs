using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using ReactiveBluetooth.Core;

namespace SampleApp.ViewModels.Central
{
    public class CharacteristicViewModel : BindableBase
    {
        private readonly ICharacteristic _characteristic;
        private Guid _uuid;

        public CharacteristicViewModel(ICharacteristic characteristic)
        {
            _characteristic = characteristic;
            _uuid = characteristic.Uuid;
        }

        public Guid Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }
    }
}
