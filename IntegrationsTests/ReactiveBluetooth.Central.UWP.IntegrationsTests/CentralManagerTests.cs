using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Shared.IntegrationsTests;
using ReactiveBluetooth.UWP.Central;

namespace ReactiveBluetooth.Central.UWP.IntegrationsTests
{
    public class CentralManagerTests : CentralTests
    {
        public override ICentralManager GetCentralManager()
        {
            return new CentralManager();
        }
    }
}
