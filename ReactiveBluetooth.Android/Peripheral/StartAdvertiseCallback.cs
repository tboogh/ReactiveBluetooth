using System;
using System.Reactive.Subjects;
using Android.Bluetooth.LE;

namespace ReactiveBluetooth.Android.Peripheral
{
    public class StartAdvertiseCallback : AdvertiseCallback
    {
        public ISubject<AdvertiseSettings> StartSuccessSubject { get; }
        public ISubject<AdvertiseFailure> StartFailureSubject { get; }

        public StartAdvertiseCallback()
        {
            StartSuccessSubject = new BehaviorSubject<AdvertiseSettings>(default(AdvertiseSettings));
            StartFailureSubject = new BehaviorSubject<AdvertiseFailure>(0);
        }

        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            StartFailureSubject?.OnNext(errorCode);
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            StartSuccessSubject?.OnNext(settingsInEffect);
        }
    }
}