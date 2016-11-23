using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Demo.Common.Behaviors;
using Demo.ViewModels.Central;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.Core.Types;
using Xamarin.Forms;

namespace Demo.ViewModels.Peripheral
{
    public class CharacteristicViewModel : BindableBase
    {
        private string _value;
        private Guid _uuid;
        private CharacteristicProperty _properties;

        public CharacteristicViewModel()
        {
            
        }

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public Guid Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public CharacteristicProperty Properties
        {
            get { return _properties; }
            set { SetProperty(ref _properties, value); }
        }

    }
}