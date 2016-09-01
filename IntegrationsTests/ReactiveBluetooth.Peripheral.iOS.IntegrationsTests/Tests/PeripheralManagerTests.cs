using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveBluetooth.Core.Peripheral;
using ReactiveBluetooth.iOS.Peripheral;

namespace ReactiveBluetooth.Peripheral.iOS.IntegrationsTests.Tests
{
    public class PeripheralManagerTests : Shared.IntegrationsTests.PeripheralManagerTests
    {
        public override IPeripheralManager GetPeripheralManager()
        {
            return new PeripheralManager();
        }
    }
}