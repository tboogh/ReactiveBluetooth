using System.Reactive.Subjects;
using Android.Bluetooth;
using Android.Content;

namespace ReactiveBluetooth.Android.Central
{
    public class BleBroadcastReciever : BroadcastReceiver
    {
        public BleBroadcastReciever()
        {
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
    }
}