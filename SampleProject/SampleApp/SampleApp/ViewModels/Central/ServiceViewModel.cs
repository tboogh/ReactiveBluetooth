using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Central;

namespace SampleApp.ViewModels.Central
{
    public class ServiceViewModel : BindableBase
    {
        private readonly IService _service;
        private Guid _uuid;
        private bool _displayCharacteristics;

        public ServiceViewModel(IService service)
        {
            _service = service;
            Uuid = _service.Uuid;
            Characteristics = new ObservableCollection<CharacteristicViewModel>();
        }

        public ObservableCollection<CharacteristicViewModel> Characteristics { get; }

        public Guid Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public bool DisplayCharacteristics
        {
            get { return _displayCharacteristics; }
            set { SetProperty(ref _displayCharacteristics, value); }
        }

        public async Task DiscoverCharacteristics()
        {
            var characteristics = await _service.DiscoverCharacteristics();
            Characteristics.Clear();

            foreach (var characteristic in characteristics)
            {
                CharacteristicViewModel characteristicViewModel = new CharacteristicViewModel(characteristic);
                Characteristics.Add(characteristicViewModel);
            }
        }
    }
}