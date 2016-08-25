using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkingNameBle.Core.Peripheral;
using WorkingNameBle.iOS.Peripheral;

namespace WorkingNameBle.Peripheral.iOS.IntegrationsTests.Tests
{
    public class PeripheralManagerTests : Shared.IntegrationsTests.PeripheralManagerTests
    {
        public override IPeripheralManager GetPeripheralManager()
        {
            return new PeripheralManager();
        }
    }
}