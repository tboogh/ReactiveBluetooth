using System;
using System.Reactive.Subjects;
using Android.Bluetooth.LE;
using ReactiveBluetooth.Core.Exceptions;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class StartAdvertiseCallback : AdvertiseCallback
    {
        public ISubject<AdvertiseSettings> AdvertiseSubject { get; }
        public StartAdvertiseCallback()
        {
            AdvertiseSubject = new BehaviorSubject<AdvertiseSettings>(default(AdvertiseSettings));
        }

        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            AdvertiseSubject?.OnError(new AdvertiseException(errorCode.ToString()));
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            AdvertiseSubject?.OnNext(settingsInEffect);
        }
    }
}