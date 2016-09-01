using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.iOS.Central;

namespace ReactiveBluetooth.Central.iOS.IntegrationTests.Tests
{
    public class CentralManagerTests : Shared.IntegrationsTests.CentralManagerTests
    {
        public override ICentralManager GetCentralManager()
        {
            return new CentralManager();
        }
    }
}