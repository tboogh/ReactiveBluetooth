using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Unity;
using SampleApp.ViewModels;
using SampleApp.ViewModels.Central;
using SampleApp.ViewModels.Peripheral;
using SampleApp.Views;
using Xamarin.Forms;

namespace SampleApp
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        protected override void OnInitialized()
        {
            NavigationService.NavigateAsync($"/{nameof(MainNavigationPage)}/{nameof(MainPage)}");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<MainNavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<PeripheralPage, PeripheralModeViewModel>();
            Container.RegisterTypeForNavigation<CentralPage, CentralModeViewModel>();
            Container.RegisterTypeForNavigation<DeviceDetailPage, DeviceViewModel>();
            Container.RegisterTypeForNavigation<CharacteristicDetailPage, CharacteristicViewModel>();
            Container.RegisterTypeForNavigation<AdvertiseDataPage, AdvertiseDataViewModel>();
        }
    }
}
