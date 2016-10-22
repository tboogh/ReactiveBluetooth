using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Navigation;
using ReactiveBluetooth.Core.Central;

namespace Demo.ViewModels.Central
{
    public class Service : BindableBase
    {
        private string _uuid;

        public string Uuid
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public Service(Guid uuid)
        {
            Uuid = uuid.ToString();
        }
    }

    public class AdvertiseDataViewModel : BindableBase, INavigationAware
    {
        private string _localName;
        private int _txPowerLevel;

        public AdvertiseDataViewModel()
        {
            ServiceUuids = new ObservableCollection<Service>();
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            IAdvertisementData advertismentData = (IAdvertisementData) parameters[nameof(IAdvertisementData)];
            LocalName = advertismentData.LocalName ?? "";
            TxPowerLevel = advertismentData.TxPowerLevel;

            if (advertismentData.ServiceUuids != null)
                foreach (var serviceUuid in advertismentData.ServiceUuids)
                {
                    ServiceUuids.Add(new Service(serviceUuid));
                }
        }

        public string LocalName
        {
            get { return _localName; }
            set { SetProperty(ref _localName, value); }
        }

        public int TxPowerLevel
        {
            get { return _txPowerLevel; }
            set { SetProperty(ref _txPowerLevel, value); }
        }

        public ObservableCollection<Service> ServiceUuids { get; }
    }
}