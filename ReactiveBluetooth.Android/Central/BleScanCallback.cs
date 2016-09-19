using System;
using System.Reactive.Subjects;
using Android.Bluetooth.LE;

namespace ReactiveBluetooth.Android.Central
{
    public class BleScanCallback : ScanCallback
    {
        public Subject<Tuple<ScanCallbackType, ScanResult>> ScanResultSubject { get; }
        public Subject<ScanFailure> FailureSubject { get; }

        public BleScanCallback()
        {
            ScanResultSubject = new Subject<Tuple<ScanCallbackType, ScanResult>>();
            FailureSubject = new Subject<ScanFailure>();
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
            FailureSubject.OnNext(errorCode);
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            ScanResultSubject.OnNext(new Tuple<ScanCallbackType, ScanResult>(callbackType, result));
        }
    }
}
