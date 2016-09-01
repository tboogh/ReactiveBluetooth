using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.Practices.Unity;
using Prism.Unity;
using WorkingNameBle.Android.Central;
using WorkingNameBle.Android.Peripheral;
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Peripheral;

namespace SampleApp.Droid
{
    [Activity(Label = "SampleApp", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new PlatformInitializer()));
        }
    }

    public class PlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IPeripheralManager, PeripheralManager>();
            container.RegisterType<ICentralManager, CentralManager>();
        }
    }
}

