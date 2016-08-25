using System;
using Android.Bluetooth.LE;

namespace WorkingNameBle.Android.Central
{
    public class BleScanCallback : ScanCallback
    {
        private readonly Action<ScanCallbackType, ScanResult> _scanResultAction;
        private readonly Action<ScanFailure> _failureAction;

        public BleScanCallback(Action<ScanCallbackType, ScanResult> scanResultAction, Action<ScanFailure> failureAction)
        {
            _scanResultAction = scanResultAction;
            _failureAction = failureAction;
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
            _failureAction?.Invoke(errorCode);
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            _scanResultAction?.Invoke(callbackType, result);
        }
    }
}
