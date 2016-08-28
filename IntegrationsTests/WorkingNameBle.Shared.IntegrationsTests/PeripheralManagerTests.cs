using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Central;
using WorkingNameBle.Core.Peripheral;

namespace WorkingNameBle.Shared.IntegrationsTests
{
    [TestFixture]
    public abstract class PeripheralManagerTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
        private IPeripheralManager _manager;
        public abstract IPeripheralManager GetPeripheralManager();

        [OneTimeSetUp]
        public async Task Setup()
        {
            _manager = GetPeripheralManager();

            var result = await _manager.Init()
                .FirstAsync(x => x == ManagerState.PoweredOn)
                .Timeout(TimeSpan.FromSeconds(2))
                .ToTask();
            Assert.AreEqual(ManagerState.PoweredOn, result);
        }

        [Test]
        public void State_PoweredOn()
        {
            Assert.AreEqual(ManagerState.PoweredOn, _manager.State);
        }

        [Test]
        public async Task StartAdvertising_StartsWithoutError()
        {
            var result = await _manager.StartAdvertising(new AdvertisingOptions {ServiceUuids = new List<Guid>() {Guid.NewGuid()}})
                .FirstAsync(b => b)
                .Timeout(Timeout);

            Assert.IsTrue(result);
        }

        [Test]
        public void AddService_DoesNotThrow()
        {
            var service = _manager.Factory.CreateService(Guid.NewGuid(), ServiceType.Primary);
            Assert.DoesNotThrow(() => _manager.AddService(service));
        }

        [Test]
        public void RemoveService_DoesNotThrow()
        {
            var service = _manager.Factory.CreateService(Guid.NewGuid(), ServiceType.Primary);
            _manager.AddService(service);
            Assert.DoesNotThrow(() => _manager.RemoveService(service));
        }

        [Test]
        public void RemoveAllService_DoesNotThrow()
        {
            var service = _manager.Factory.CreateService(Guid.NewGuid(), ServiceType.Primary);
            _manager.AddService(service);
            Assert.DoesNotThrow(() => _manager.RemoveAllServices());
        }
    }
}