using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Prism.Mvvm;
using ReactiveBluetooth.Core.Peripheral;

namespace Demo.ViewModels.Peripheral
{
    public class ServiceViewModel : BindableBase
    {
        private Guid _uuid;

        public Guid Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }
    }
}