using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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

            var scanObservable = bluetoothService.ScanForDevices().Timeout(TimeSpan.FromSeconds(20));
            var scanDisposable = scanObservable.Subscribe(device =>
            {
                devices.Add(device);
            });

            await Task.Delay(TimeSpan.FromSeconds(20));

            Assert.IsNotEmpty(devices);
            scanDisposable.Dispose();
        }
    }
}
