using System;
using System.Collections.Generic;
using System.Linq;
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
            
        }

        [Test]
        public void ReadyToDiscover_DeviceReadyToDiscover_ReturnsTrue()
        {
            
        }

        [Test]
        public void ScanForDevices_DiscoversDevices_CompletesInTwentySeconds()
        {

        }
    }
}
