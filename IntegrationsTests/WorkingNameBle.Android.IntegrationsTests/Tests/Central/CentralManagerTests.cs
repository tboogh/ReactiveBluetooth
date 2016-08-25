using WorkingNameBle.Android.Central;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Android.IntegrationsTests.Tests.Central
{
    public class CentralManagerTests : Shared.IntegrationsTests.CentralManagerTests {
        public override ICentralManager GetCentralManager()
        {
            return new CentralManager();
        }
    }
}