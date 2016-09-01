using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Unity;
using SampleApp.ViewModels;
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
            NavigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(MainPage)}");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MainPage>();
            Container.RegisterTypeForNavigation<PeripheralPage, PeripheralModeViewModel>();
            Container.RegisterTypeForNavigation<CentralPage, CentralModeViewModel>();
        }
    }
}
