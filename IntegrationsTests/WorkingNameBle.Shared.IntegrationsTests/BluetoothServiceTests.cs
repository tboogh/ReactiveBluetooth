using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using WorkingNameBle.Core;

namespace WorkingNameBle.Shared.IntegrationsTests
{
    [TestFixture]
    public abstract class BluetoothServiceTests
    {
        public abstract IBluetoothService GetService();

        [Test]
        public void Init_CompleteInitialization_NoExceptions()
        {
            var bluetoothService = GetService();
            Assert.DoesNotThrow(() => bluetoothService.Init());
        }

        [Test]
        public async Task ReadyToDiscover_DeviceReadyToDiscover_ReturnsTrue()
        {
            var bluetoothService = GetService();
            bluetoothService.Init();
            var ready = await bluetoothService.ReadyToDiscover();

            Assert.IsTrue(ready);
        }

        [Test]
        public async Task ScanForDevices_DiscoversDevicesCompletesInTwentySeconds_DeviceListNotEmpty()
        {
            var bluetoothService = GetService();
            bluetoothService.Init();

            var ready = await bluetoothService.ReadyToDiscover();
            if (!ready)
            {
                throw new Exception("Device not ready");
            }

            List<IDevice> devices = new List<IDevice>();

            var scanObservable = bluetoothService.ScanForDevices().Timeout(TimeSpan.FromSeconds(5));
            var scanDisposable = scanObservable.Subscribe(device =>
            {
                devices.Add(device);
            });

            await Task.Delay(TimeSpan.FromSeconds(5));

            Assert.IsNotEmpty(devices);
            scanDisposable.Dispose();
        }

        [Test]
        public async Task ScanForDevices_FindTestDevice_NotNull()
        {
            var bluetoothService = GetService();
            bluetoothService.Init();

            var ready = await bluetoothService.ReadyToDiscover();
            if (!ready)
            {
                throw new Exception("Device not ready");
            }

            var scanObservable = bluetoothService.ScanForDevices();
            var battByte = await scanObservable.FirstAsync(x => x.Name == "BleTest").Timeout(TimeSpan.FromSeconds(20)).ToTask();

            Assert.NotNull(battByte);
        }

        [Test]
        public async Task ConnectToDevice_ConnectToTestDevice_ResultIsTrue()
        {
            var bluetoothService = GetService();
            bluetoothService.Init();

            var ready = await bluetoothService.ReadyToDiscover();
            if (!ready)
            {
                throw new Exception("Device not ready");
            }

            var scanObservable = bluetoothService.ScanForDevices();
            var battByte = await scanObservable.FirstAsync(x => x.Name == "BleTest").Timeout(TimeSpan.FromSeconds(20)).ToTask();

            if (battByte == null)
            {
                throw new Exception("Make sure test device is available");
            }

            var connectionResult = await bluetoothService.ConnectToDevice(battByte);

            Assert.IsTrue(connectionResult);

            await bluetoothService.DisconnectDevice(battByte);
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


            var services = await device.DiscoverServices().Timeout(TimeSpan.FromSeconds(20)).ToTask();
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


            var services = await device.DiscoverServices().Timeout(TimeSpan.FromSeconds(20)).ToTask();
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

        private async Task<Tuple<IBluetoothService, IDevice>> ConnectToTestDevice()
        {
            var bluetoothService = GetService();
            bluetoothService.Init();

            var ready = await bluetoothService.ReadyToDiscover();
            if (!ready)
            {
                throw new Exception("Device not ready");
            }

            var scanObservable = bluetoothService.ScanForDevices();
            var testDevice = await scanObservable.FirstAsync(x => x.Name == "BleTest")
                .Timeout(TimeSpan.FromSeconds(20))
                .ToTask();

            if (testDevice == null)
            {
                throw new Exception("Make sure test device is available");
            }

            await bluetoothService.ConnectToDevice(testDevice);
            return new Tuple<IBluetoothService, IDevice>(bluetoothService, testDevice);
        }
    }
}
