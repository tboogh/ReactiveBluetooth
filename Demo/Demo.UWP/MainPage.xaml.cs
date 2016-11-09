using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.Unity;
using Prism.Unity;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.UWP.Central;

namespace Demo.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Demo.App(new PlatformInitializer()));
        }
    }

    public class PlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            //container.RegisterType<IPeripheralManager, PeripheralManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICentralManager, CentralManager>(new ContainerControlledLifetimeManager());
        }
    }
}
