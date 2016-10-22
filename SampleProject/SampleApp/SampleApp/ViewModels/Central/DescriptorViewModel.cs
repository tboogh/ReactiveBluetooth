using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using ReactiveBluetooth.Core;

namespace SampleApp.ViewModels.Central
{
    public class DescriptorViewModel : BindableBase
    {
        IDescriptor _descriptor;

        public DescriptorViewModel(IDescriptor descriptor)
        {
            UpdateDescriptor(descriptor);
        }

        public Guid Uuid => _descriptor?.Uuid ?? Guid.Empty;

        private void UpdateDescriptor(IDescriptor descriptor)
        {
            _descriptor = descriptor;
            OnPropertyChanged(() => Uuid);
        }
    }
}
