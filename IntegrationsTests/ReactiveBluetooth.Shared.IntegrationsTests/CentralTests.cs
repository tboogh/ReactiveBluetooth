using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;

namespace ReactiveBluetooth.Shared.IntegrationsTests
{
    [TestFixture]
    public abstract class CentralTests
    {
        private static readonly string TestDeviceName = "TP";
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

        private ICentralManager _centralManager;
        public abstract ICentralManager GetCentralManager();

        [OneTimeSetUp]
        public async Task SetupManager()
        {
            _centralManager = GetCentralManager();
            await _centralManager.State()
                .FirstAsync(state => state == ManagerState.PoweredOn)
                .Timeout(Timeout);
        }

        [Test]
        public async Task ManagerState_IsPoweredOn()
        {
            var currentState = await _centralManager.State()
                .FirstAsync(state => state == ManagerState.PoweredOn)
                .Timeout(Timeout);
            Assert.AreEqual(ManagerState.PoweredOn, currentState);
        }

        [Test]
        public async Task ScanForDevices_DiscoversDevicesFindAny_NotNull()
        {
           
            var device = await _centralManager.ScanForDevices().FirstAsync().Timeout(Timeout);
            
            await Task.Delay(TimeSpan.FromSeconds(5));

            Assert.NotNull(device);
        }

        [Test]
        public async Task ScanForDevices_FindTestDevice_NotNull()
        {

            var scanObservable = _centralManager.ScanForDevices();
            var battByte = await scanObservable.FirstAsync(x =>
            {
                return x.Name == TestDeviceName;
            }).Timeout(Timeout).ToTask();

            Assert.NotNull(battByte);
        }

        [Test]
        public async Task ConnectToDevice_ConnectToTestDevice_ResultIsTrue()
        {
            var scanObservable = _centralManager.ScanForDevices();
            var battByte = await scanObservable.FirstAsync(x => x.Name == TestDeviceName).Timeout(Timeout).ToTask();

            if (battByte == null)
            {
                throw new Exception("Make sure test device is available");
            }

            var connectionResult = await _centralManager.ConnectToDevice(battByte).FirstAsync(x => x == ConnectionState.Connected).Timeout(TimeSpan.FromSeconds(2));

            Assert.AreEqual(ConnectionState.Connected, connectionResult);

            await _centralManager.DisconnectDevice(battByte);
        }

        [Test]
        public async Task ConnectToDevice_ConnectToTestDevice_StateIsConnected()
        {
            var testSetup = await ConnectToTestDevice();
            var service = testSetup.Item1;
            var device = testSetup.Item2;

            Assert.AreEqual(ConnectionState.Connected, device.State);
            await service.DisconnectDevice(device);
        }

        [Test]
        public async Task DisconnectDevice_DisconnectFromTestDevice_StateIsDisconnected()
        {
            var testSetup = await ConnectToTestDevice();
            var service = testSetup.Item1;
            var device = testSetup.Item2;
            
            await service.DisconnectDevice(device);

            Assert.AreEqual(ConnectionState.Disconnected, device.State);
        }

        [Test]
        public async Task DiscoverServices_FindServicesOnTestDevice_ListNotNull()
        {
            var testSetup = await ConnectToTestDevice();
            var service = testSetup.Item1;
            var device = testSetup.Item2;


            var services = await device.DiscoverServices().Timeout(Timeout)
                .FirstAsync();
            await service.DisconnectDevice(device);

            Assert.NotNull(services);
            Assert.IsNotEmpty(services);
        }

        [Test]
        public async Task DiscoverServices_FindServicesOnTestDevice_ServiceIdSet()
        {
            var testSetup = await ConnectToTestDevice();
            var service = testSetup.Item1;
            var device = testSetup.Item2;


            var services = await device.DiscoverServices().Timeout(Timeout)
                .FirstAsync();
            await service.DisconnectDevice(device);

            if (services.Count == 0)
            {
                throw new Exception("Check test device services");
            }

            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            Assert.NotNull(testService?.Uuid);
        }

        [Test]
        public async Task DiscoverCharacteristics_FindCharacteristicsOnService_NotNullList()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;


            var services = await device.DiscoverServices().Timeout(Timeout).FirstAsync();
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            var characteristics = await testService.DiscoverCharacteristics().Timeout(Timeout).FirstAsync();

            var testCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"));

            await testSetup.Item1.DisconnectDevice(device);

            Assert.NotNull(testCharacterstic);
        }

        [Test]
        public async Task ReadValue_CharactersiticValue_CorrectValue()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;


            var services = await device.DiscoverServices().Timeout(Timeout).FirstAsync();
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            var characteristics = await testService.DiscoverCharacteristics().Timeout(Timeout).FirstAsync();

            var testCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"));

            await testSetup.Item1.DisconnectDevice(device);

            var value = await device.ReadValue(testCharacterstic).Timeout(Timeout).FirstAsync();
            Assert.AreEqual(new byte[] { 0xB0, 0x06 }, value);
        }

        private async Task<Tuple<ICentralManager, IDevice>> ConnectToTestDevice()
        {
            var scanObservable = _centralManager.ScanForDevices();
            var testDevice = await scanObservable.FirstAsync(x => x.Name == TestDeviceName)
                .Timeout(Timeout)
                .ToTask();

            if (testDevice == null)
            {
                throw new Exception("Make sure test device is available");
            }

            var connectionResult = await _centralManager.ConnectToDevice(testDevice).FirstAsync(x => x == ConnectionState.Connected).Timeout(TimeSpan.FromSeconds(2));
            return new Tuple<ICentralManager, IDevice>(_centralManager, testDevice);
        }
    }
}
