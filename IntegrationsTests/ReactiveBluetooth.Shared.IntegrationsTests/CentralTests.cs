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
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

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
            var device = await _centralManager.ScanForDevices(new List<Guid>() { Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62") }).FirstAsync().Timeout(Timeout);
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

            var connectionResult = await _centralManager.Connect(testDevice).FirstAsync(x => x == ConnectionState.Connected).Timeout(Timeout);
            _centralManager.Disconnect(testDevice);
            Assert.AreEqual(ConnectionState.Connected, connectionResult);
        }

        [Test]
        public async Task ConnectToDevice_ConnectToTestDevice_StateIsConnected()
        {
            var device = await FindTestDevice();
            ConnectionState state = await _centralManager.Connect(device).FirstAsync(x => x == ConnectionState.Connected).Timeout(Timeout);

            _centralManager.Disconnect(device);
            Assert.AreEqual(ConnectionState.Connected, state);
        }

        [Test]
        public async Task DisconnectDevice_DisconnectFromTestDevice_StateIsDisconnected()
        {
            var device = await ConnectToTestDevice();
            _centralManager.Disconnect(device);
            await Task.Delay(TimeSpan.FromSeconds(30));
            Assert.AreEqual(ConnectionState.Disconnected, device.State);
        }

        [Test]
        public async Task DiscoverServices_FindServicesOnTestDevice_ListNotNull()
        {
            var device = await ConnectToTestDevice();
            
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            _centralManager.Disconnect(device);
            Assert.NotNull(services);
            Assert.IsNotEmpty(services);
        }

        [Test]
        public async Task DiscoverServices_FindServicesOnTestDevice_ServiceIdSet()
        {
            var device = await ConnectToTestDevice();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            cancellationTokenSource.Token.Register(() =>
            { _centralManager.Disconnect(device); });

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            if (services.Count == 0)
            {
                throw new Exception("Check test device services");
            }

            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));
            _centralManager.Disconnect(device);
            Assert.NotNull(testService?.Uuid);
        }

        [Test]
        public async Task DiscoverCharacteristics_FindCharacteristicsOnService_NotNullList()
        {
            var device = await ConnectToTestDevice();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            cancellationTokenSource.Token.Register(() =>
            { _centralManager.Disconnect(device); });

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var testCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"));
            _centralManager.Disconnect(device);
            Assert.NotNull(testCharacterstic);
        }

        [Test]
        public async Task ReadValue_CorrectValue()
        {
            var device = await ConnectToTestDevice();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            cancellationTokenSource.Token.Register(() =>
            { _centralManager.Disconnect(device); });

            var services = await device.DiscoverServices(CancellationToken.None);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var testCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060001-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var value = await device.ReadValue(testCharacterstic, cancellationTokenSource.Token);

            _centralManager.Disconnect(device);
            Assert.AreEqual(new byte[] { 0xB0, 0x0B }, value);
        }

        [Test]
        public async Task WriteValue_WithResponse()
        {
            var device = await ConnectToTestDevice();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            cancellationTokenSource.Token.Register(() =>
            { _centralManager.Disconnect(device); });

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060002-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var success = await device.WriteValue(writeCharacterstic, new byte[] { 0x12, 0x34, 0x56, 0x78 }, WriteType.WithResponse, cancellationTokenSource.Token);

            _centralManager.Disconnect(device);
            Assert.IsTrue(success);
        }

        [Test]
        public async Task WriteValue_WithoutResponse()
        {
            var device = await ConnectToTestDevice();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            cancellationTokenSource.Token.Register(() =>
            { _centralManager.Disconnect(device); });

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060003-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var sucess = await device.WriteValue(writeCharacterstic, new byte[] { 0x12, 0x34, 0x56, 0x78 }, WriteType.WithoutRespoonse, cancellationTokenSource.Token);
            _centralManager.Disconnect(device);
            Assert.IsTrue(sucess);
        }

        [Test]
        public async Task WriteValue_WithResponse_CorrectValue()
        {
            var device = await ConnectToTestDevice();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            cancellationTokenSource.Token.Register(() =>
            { _centralManager.Disconnect(device); });
            
            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060002-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var sucess = await device.WriteValue(writeCharacterstic, new byte[] { 0x12, 0x34, 0x56, 0x78 }, WriteType.WithResponse, cancellationTokenSource.Token);

            cancellationTokenSource.CancelAfter(Timeout);
            var value = await device.ReadValue(writeCharacterstic, cancellationTokenSource.Token);
            _centralManager.Disconnect(device);
            Assert.AreEqual(new byte[] { 0x12, 0x34, 0x56, 0x78 }, value);
        }

        [Test]
        public async Task WriteValue_WithoutResponse_CorrectValue()
        {
            var device = await ConnectToTestDevice();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(Timeout);
            cancellationTokenSource.Token.Register(() =>
            { _centralManager.Disconnect(device); });

            var services = await device.DiscoverServices(cancellationTokenSource.Token);
            var testService = services.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var characteristics = await testService.DiscoverCharacteristics(cancellationTokenSource.Token);

            var writeCharacterstic = characteristics.FirstOrDefault(x => x.Uuid == Guid.Parse("B0060003-0234-49D9-8439-39100D7EBD62"));

            cancellationTokenSource.CancelAfter(Timeout);
            var sucess = await device.WriteValue(writeCharacterstic, new byte[] { 0x12, 0x34, 0x56, 0x78 }, WriteType.WithoutRespoonse, cancellationTokenSource.Token);

            cancellationTokenSource.CancelAfter(Timeout);
            var value = await device.ReadValue(writeCharacterstic, cancellationTokenSource.Token);
            _centralManager.Disconnect(device);
            Assert.AreEqual(new byte[] { 0x12, 0x34, 0x56, 0x78 }, value);
        }



        private async Task<IDevice> ConnectToTestDevice()
        {
            IDevice device = await FindTestDevice();
            var connectionObservable = await _centralManager.Connect(device).FirstAsync(x => x == ConnectionState.Connected).Timeout(Timeout);
            return device;
        }

        private async Task<IDevice> FindTestDevice()
        {
            return await _centralManager.ScanForDevices(new List<Guid>() { Guid.Parse("B0060000-0234-49D9-8439-39100D7EBD62") }).FirstAsync().Timeout(Timeout);
        }
    }
}