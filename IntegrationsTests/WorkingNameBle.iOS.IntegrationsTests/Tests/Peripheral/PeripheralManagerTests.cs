using System;
using System.Collections.Generic;
using System.Text;
using WorkingNameBle.Core.Peripheral;
using WorkingNameBle.iOS.Peripheral;

namespace WorkingNameBle.iOS.IntegrationsTests.Tests.Peripheral
{
    public class PeripheralManagerTests : Shared.IntegrationsTests.PeripheralManagerTests
    {
        public override IPeripheralManager GetPeripheralManager()
        {
            return new PeripheralManager();
        }
    }
}
