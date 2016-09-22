using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using ReactiveBluetooth.Core.Central;
using UIKit;

namespace Issue_14
{
    public partial class CoreTableViewController : UITableViewController
    {
        public CoreTableViewController (IntPtr handle) : base (handle)
        {
        }

        private List<IDevice> _devices;
        private ReactiveBluetooth.iOS.Central.CentralManager _centralManager;
        private IDisposable _scanDisp;
        private IDisposable _connectDisp;
        private IDisposable _stateDisp;
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _devices = new List<IDevice>();
            _centralManager = new ReactiveBluetooth.iOS.Central.CentralManager();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _stateDisp = _centralManager.State().SubscribeOn(SynchronizationContext.Current).Subscribe(state =>
            {
                if (state == ReactiveBluetooth.Core.Central.ManagerState.PoweredOn)
                {
                    _scanDisp?.Dispose();
                    _scanDisp = _centralManager.ScanForDevices().Distinct(x => x.Uuid).SubscribeOn(SynchronizationContext.Current).Subscribe(device =>
                    {
                        _devices.Add(device);
                        TableView.ReloadData();
                    });
                }
            });
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            _scanDisp?.Dispose();
            _connectDisp?.Dispose();
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return _devices.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("PCell", indexPath);
            var device = _devices[indexPath.Row];
            cell.TextLabel.Text = device.Name;
            cell.DetailTextLabel.Text = device.Uuid.ToString();
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _connectDisp?.Dispose();
            _connectDisp = _centralManager.ConnectToDevice(_devices[indexPath.Row]).SubscribeOn(SynchronizationContext.Current).Subscribe(state =>
            {
                Debug.WriteLine(state.ToString());
                //UIAlertController controller = UIAlertController.Create("Connection changed", state.ToString(), UIAlertControllerStyle.Alert);
                //controller.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, action =>
                //{
                //    DismissViewController(true, null);
                //}));
                //PresentViewController(controller, true, () =>
                //{

                //});
            });
        }
    }
}