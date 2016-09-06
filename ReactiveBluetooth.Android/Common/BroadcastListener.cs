using System;
using System.Reactive.Subjects;
using Android.Bluetooth;
using Android.Content;
using Plugin.CurrentActivity;

namespace ReactiveBluetooth.Android.Common
{
    public class BroadcastListener : BroadcastReceiver
    {
        public BroadcastListener()
        {
            CrossCurrentActivity.Current.Activity.RegisterReceiver(this, new IntentFilter(BluetoothAdapter.ActionConnectionStateChanged));
            StateUpdatedSubject = new BehaviorSubject<State>(BluetoothAdapter.DefaultAdapter.State);
        }


        public BehaviorSubject<State> StateUpdatedSubject { get; }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            if (action == BluetoothAdapter.ActionStateChanged)
            {
                State state = (State) intent.GetIntExtra(BluetoothAdapter.ExtraState, BluetoothAdapter.Error);
                StateUpdatedSubject?.OnNext(state);
            }
        }

        protected override void Dispose(bool disposing)
        {
            CrossCurrentActivity.Current.Activity.UnregisterReceiver(this);
            base.Dispose(disposing);
        }
    }
}