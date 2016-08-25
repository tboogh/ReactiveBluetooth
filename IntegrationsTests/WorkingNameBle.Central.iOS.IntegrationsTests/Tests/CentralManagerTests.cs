using WorkingNameBle.Core.Central;
using WorkingNameBle.iOS.Central;

namespace WorkingNameBle.Central.iOS.IntegrationTests.Tests
{
    public class CentralManagerTests : Shared.IntegrationsTests.CentralManagerTests
    {
        public override ICentralManager GetCentralManager()
        {
            return new CentralManager();
        }
    }
}