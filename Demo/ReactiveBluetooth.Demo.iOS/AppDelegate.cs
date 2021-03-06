﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Microsoft.Practices.Unity;
using Prism.Unity;
using UIKit;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.iOS.Central;
using ReactiveBluetooth.iOS.Peripheral;

namespace Demo.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new PlatformInitializer()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class PlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IPeripheralManager, PeripheralManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICentralManager, CentralManager>(new ContainerControlledLifetimeManager());
        }
    }
}
