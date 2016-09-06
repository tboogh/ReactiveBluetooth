using System;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.Practices.Unity;
using Plugin.CurrentActivity;
using Prism.Unity;
using ReactiveBluetooth.Android.Central;
using ReactiveBluetooth.Android.Common;
using ReactiveBluetooth.Android.Peripheral;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Peripheral;

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
            var app = new App(new PlatformInitializer());
            LoadApplication(app);
        }
    }

    public class PlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IPeripheralManager, PeripheralManager>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<ICentralManager, CentralManager>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
        }
    }
}

