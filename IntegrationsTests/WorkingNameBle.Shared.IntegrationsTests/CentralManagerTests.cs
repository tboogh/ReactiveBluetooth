using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using WorkingNameBle.Core;
using WorkingNameBle.Core.Central;

namespace WorkingNameBle.Shared.IntegrationsTests
{
    [TestFixture]
    public abstract class CentralManagerTests
    {
        private static readonly string TestDeviceName = "BleTest";
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2);

        private ICentralManager _centralManager;
        public abstract ICentralManager GetCentralManager();

        [OneTimeSetUp]
        public async Task SetupManager()
        {
            _centralManager = GetCentralManager();
            await _centralManager.Init()
                .FirstAsync(state => state == ManagerState.PoweredOn)
                .Timeout(Timeout);
        }

        [Test]
        public void ManagerState_IsPoweredOn()
        {
            Assert.AreEqual(ManagerState.PoweredOn, _centralManager.State);
        }

        [Test]
        public void Init_CompleteInitialization_NoExceptions()
        {
            var bluetoothService = GetCentralManager();
            Assert.DoesNotThrow(() => bluetoothService.Init());
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
            var battByte = await scanObservable.FirstAsync(x => x.Name == TestDeviceName).Timeout(Timeout).ToTask();

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

            var connectionResult = await _centralManager.ConnectToDevice(battByte);

            Assert.IsTrue(connectionResult);

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


            var services = await device.DiscoverServices().Timeout(Timeout).ToTask();
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


            var services = await device.DiscoverServices().Timeout(Timeout).ToTask();
            await service.DisconnectDevice(device);

            if (services.Count == 0)
            {
                throw new Exception("Check test device services");
            }

            var testService = services.First();

            Assert.NotNull(testService.Id);
        }

        [Test]
        public async Task DiscoverCharacteristics_FindCharacteristicsOnService_NotNullList()
        {
            var testSetup = await ConnectToTestDevice();
            var service = testSetup.Item1;
            var device = testSetup.Item2;

            // 180A
            // 0000-1000-8000-00805F9B34FB

            throw new NotImplementedException("Test incomplete");
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

            await _centralManager.ConnectToDevice(testDevice);
            return new Tuple<ICentralManager, IDevice>(_centralManager, testDevice);
        }
    }
}
