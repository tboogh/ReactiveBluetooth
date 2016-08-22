using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingNameBle.Core;

namespace WorkingNameBle.iOS.IntegrationsTests
{
    public class BluetoothServiceTests : Shared.IntegrationsTests.BluetoothServiceTests
    {
        public override IBluetoothService GetService()
        {
            return new BluetoothService();
        }
    }
}