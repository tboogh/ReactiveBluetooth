using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;
using ReactiveBluetooth.Core.Types;

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
            Assert.NotNull(device);
        }

        [Test]
        public async Task ScanForDevices_DiscoversDevicesServiceFilterFindAny_NotNull()
        {
            var device = await _centralManager.ScanForDevices().FirstAsync().Timeout(Timeout);
            Assert.NotNull(device);
        }

        [Test]
        public async Task ScanForDevices_FindTestDevice_NotNull()
        {
            var scanObservable = _centralManager.ScanForDevices(new List<Guid>() { Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62") });
            var testDevice = await scanObservable.FirstAsync().Timeout(Timeout).ToTask();

            Assert.NotNull(testDevice);
            Assert.AreEqual(TestDeviceName, testDevice.Name);
        }

        [Test]
        public async Task ConnectToDevice_ConnectToTestDevice_ResultIsTrue()
        {
            var scanObservable = _centralManager.ScanForDevices(new List<Guid>() { Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62") });
            var testDevice = await scanObservable.FirstAsync().Timeout(Timeout).ToTask();

            if (testDevice == null)
            {
                throw new Exception("Make sure test device is available");
            }

            var connectionResult = await _centralManager.ConnectToDevice(testDevice).FirstAsync(x => x == ConnectionState.Connected).Timeout(TimeSpan.FromSeconds(2));

            Assert.AreEqual(ConnectionState.Connected, connectionResult);
        }

        [Test]
        public async Task ConnectToDevice_ConnectToTestDevice_StateIsConnected()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;

            Assert.AreEqual(ConnectionState.Connected, device.State);        }

        [Test]
        public async Task DisconnectDevice_DisconnectFromTestDevice_StateIsDisconnected()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;
            
            Assert.AreEqual(ConnectionState.Disconnected, device.State);
        }

        [Test]
        public async Task DiscoverServices_FindServicesOnTestDevice_ListNotNull()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            Assert.NotNull(services);
            Assert.IsNotEmpty(services);
        }

        [Test]
        public async Task DiscoverServices_FindServicesOnTestDevice_ServiceIdSet()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var services = await device.DiscoverServices(cancellationTokenSource.Token);
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

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var testCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"));
            Assert.NotNull(testCharacterstic);
        }

        [Test]
        public async Task ReadValue_CorrectValue()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var services = await device.DiscoverServices(CancellationToken.None);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var testCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var value = await device.ReadValue(testCharacterstic, cancellationTokenSource.Token);
            Assert.AreEqual(new byte[] { 0xB0, 0x0B }, value);
        }

        [Test]
        public async Task WriteValue_WithResponse()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060002-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var sucess = await device.WriteValue(writeCharacterstic, new byte[] {0x12, 0x34, 0x56, 0x78}, WriteType.WithResponse, cancellationTokenSource.Token);
            Assert.IsTrue(sucess);
        }

        [Test]
        public async Task WriteValue_WithoutResponse()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060003-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var sucess = await device.WriteValue(writeCharacterstic, new byte[] { 0x12, 0x34, 0x56, 0x78 }, WriteType.WithoutRespoonse, cancellationTokenSource.Token);
            Assert.IsTrue(sucess);
        }

        [Test]
        public async Task WriteValue_WithResponse_CorrectValue()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;
            
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060002-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var sucess = await device.WriteValue(writeCharacterstic, new byte[] { 0x12, 0x34, 0x56, 0x78 }, WriteType.WithResponse, cancellationTokenSource.Token);

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var value = await device.ReadValue(writeCharacterstic, cancellationTokenSource.Token);

            Assert.AreEqual(new byte[] { 0x12, 0x34, 0x56, 0x78 }, value);
        }

        [Test]
        public async Task WriteValue_WithoutResponse_CorrectValue()
        {
            var testSetup = await ConnectToTestDevice();
            var device = testSetup.Item2;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060003-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var sucess = await device.WriteValue(writeCharacterstic, new byte[] { 0x12, 0x34, 0x56, 0x78 }, WriteType.WithoutRespoonse, cancellationTokenSource.Token);

            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(15));
            var value = await device.ReadValue(writeCharacterstic, cancellationTokenSource.Token);

            Assert.AreEqual(new byte[] { 0x12, 0x34, 0x56, 0x78 }, value);
        }

        private async Task<Tuple<ICentralManager, IDevice>> ConnectToTestDevice()
        {
            var scanObservable = _centralManager.ScanForDevices(new List<Guid>() { Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62") });
            var testDevice = await scanObservable.FirstAsync()
                .Timeout(Timeout)
                .ToTask();

            if (testDevice == null)
            {
                throw new Exception("Make sure test device is available");
            }

            ConnectionState connectionResult = await _centralManager.ConnectToDevice(testDevice).FirstAsync(x => x == ConnectionState.Connected).Timeout(TimeSpan.FromSeconds(2));

            return new Tuple<ICentralManager, IDevice>(_centralManager, testDevice);
        }
    }
}
