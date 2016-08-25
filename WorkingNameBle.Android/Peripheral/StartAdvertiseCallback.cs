using System;
using Android.Bluetooth.LE;

namespace WorkingNameBle.Android.Peripheral
{
    public class StartAdvertiseCallback : AdvertiseCallback
    {
        public Action<bool> StartSuccess { get; set; }
        public Action<AdvertiseFailure> StartFailure { get; set; }

        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            base.OnStartFailure(errorCode);
            StartFailure?.Invoke(errorCode);
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            base.OnStartSuccess(settingsInEffect);
            StartSuccess?.Invoke(true);
        }
    }
}