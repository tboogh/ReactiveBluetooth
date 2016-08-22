using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkingNameBle.Core;

namespace WorkingNameBle.Android.IntegrationsTests
{
    public class BluetoothServiceTests : Shared.IntegrationsTests.BluetoothServiceTests {
        public override IBluetoothService GetService()
        {
            return new BluetoothService();
        }
    }
}