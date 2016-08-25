using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.Shared.IntegrationsTests
{
    [TestFixture]
    public abstract class PeripheralManagerTests
    {
        private IPeripheralManager _manager;
        public abstract IPeripheralManager GetPeripheralManager();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _manager = GetPeripheralManager();

            await _manager.Init()
                .FirstAsync(x => x == ManagerState.PoweredOn)
                .Timeout(TimeSpan.FromSeconds(5));
        }

        [Test]
        public void State_PoweredOn()
        {
            Assert.AreEqual(ManagerState.PoweredOn, _manager.State);
        }
    }
}
