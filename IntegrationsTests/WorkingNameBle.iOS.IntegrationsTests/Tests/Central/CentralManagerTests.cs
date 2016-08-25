using WorkingNameBle.Core.Central;
using WorkingNameBle.iOS.Central;

namespace WorkingNameBle.iOS.IntegrationsTests.Tests.Central
{
    public class CentralManagerTests : Shared.IntegrationsTests.CentralManagerTests
    {
        public override ICentralManager GetCentralManager()
        {
            return new CentralManager();
        }
    }
}