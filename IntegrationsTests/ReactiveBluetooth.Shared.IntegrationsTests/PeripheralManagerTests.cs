using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Peripheral;
using IService = ReactiveBluetooth.Core.Peripheral.IService;

namespace ReactiveBluetooth.Shared.IntegrationsTests
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

            var result = await _manager.State()
                .FirstAsync(x => x == ManagerState.PoweredOn)
                .Timeout(TimeSpan.FromSeconds(2));
            Assert.AreEqual(ManagerState.PoweredOn, result);
        }

        [Test]
        public async Task State_PoweredOn()
        {
            var result = await _manager.State()
                .FirstAsync(x => x == ManagerState.PoweredOn)
                .Timeout(TimeSpan.FromSeconds(2));
            Assert.AreEqual(ManagerState.PoweredOn, result);
        }

        [Test]
        public async Task StartAdvertising_StartsWithoutError()
        {
            var result = await _manager.Advertise(new AdvertisingOptions {ServiceUuids = new List<Guid>() {Guid.NewGuid()}}, new List<IService>())
                .FirstAsync(b => b)
                .Timeout(Timeout);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task StartAdvertising_TwoCalls_DoesNotThrow()
        {
            var result = _manager.Advertise(new AdvertisingOptions {ServiceUuids = new List<Guid>() {Guid.NewGuid()}}, new List<IService>())
                .Subscribe(b =>
                {

                });

            bool threw = false;
            try
            {
                var result2 = await _manager.Advertise(new AdvertisingOptions {ServiceUuids = new List<Guid>() {Guid.NewGuid()}}, new List<IService>())
                    .FirstAsync(b => b)
                    .Timeout(Timeout);
            }
            catch (Exception e)
            {
                threw = true;
            }
            Assert.False(threw);
            result.Dispose();
        }

        [Test]
        public async Task StartAdvertising_StartStopStartAdvertise_NoError()
        {
            bool threw = false;
            try
            {
                var result = await _manager.Advertise(new AdvertisingOptions { ServiceUuids = new List<Guid>() { Guid.NewGuid() } }, new List<IService>())
                                .FirstAsync(b => b)
                                .Timeout(Timeout);

                var result2 = await _manager.Advertise(new AdvertisingOptions { ServiceUuids = new List<Guid>() { Guid.NewGuid() } }, new List<IService>())
                    .FirstAsync(b => b)
                    .Timeout(Timeout);
            } catch (Exception e)
            {
                threw = true;
            }
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