using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.iOS.Central;

namespace ReactiveBluetooth.Central.iOS.IntegrationTests.Tests
{
    public class CentralTests : Shared.IntegrationsTests.CentralTests
    {
        public override ICentralManager GetCentralManager()
        {
            return new CentralManager();
        }
    }
}