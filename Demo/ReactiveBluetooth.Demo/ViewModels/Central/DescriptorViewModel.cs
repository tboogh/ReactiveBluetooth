using System;
using Prism.Mvvm;
using ReactiveBluetooth.Core;

namespace Demo.ViewModels.Central
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
