using Foundation;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using UIKit;

namespace Issue_14
{
    public partial class TableViewController : UITableViewController
    {
        private List<Device> _devices;
        private CentralManager _centralManager;
        private IDisposable _scanDisp;
        private IDisposable _connectDisp;

        public TableViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _devices = new List<Device>();
            _centralManager = new CentralManager();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _scanDisp = _centralManager.ScanForDevices().Distinct(x => x.Uuid).SubscribeOn(SynchronizationContext.Current).Subscribe(device =>
            {
                _devices.Add(device);
                TableView.ReloadData();
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
            base.RowSelected(tableView, indexPath);
            _connectDisp?.Dispose();
            _connectDisp = _centralManager.ConnectToDevice(_devices[indexPath.Row]).SubscribeOn(SynchronizationContext.Current).Subscribe(state =>
            {
                UIAlertController  controller = UIAlertController.Create("Connection changed", state.ToString(), UIAlertControllerStyle.Alert);
                PresentViewController(controller, true, () =>
                {
                    
                });
            });
        }
    }
}