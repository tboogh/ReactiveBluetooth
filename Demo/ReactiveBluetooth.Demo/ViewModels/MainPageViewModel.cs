using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReactiveBluetooth.Core;
using ReactiveBluetooth.Core.Central;

namespace Demo.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;
        private IDisposable _stateDisposable;

        public MainPageViewModel(INavigationService navigationService, ICentralManager centralManager, IPageDialogService pageDialogService)
        {
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;
            DisplayPeripheralPageCommand = DelegateCommand.FromAsyncHandler(DisplayPeripheralPage);
            DisplayCentralPageCommand = DelegateCommand.FromAsyncHandler(DisplayCentralPage);

            _stateDisposable = centralManager.State()
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(async state =>
                {
                    switch (state)
                    {
                        case ManagerState.Unknown:
                            break;
                        case ManagerState.Resetting:
                            break;
                        case ManagerState.Unsupported:
                            await _pageDialogService.DisplayAlertAsync("Not suppored", "BLE is not supported on this device", "I understand");
                            break;
                        case ManagerState.Unauthorized:
                            await _pageDialogService.DisplayAlertAsync("Bluetooth error", "Not authorized to use bluetooth", "Ok");
                            break;
                        case ManagerState.PoweredOff:
                            await _pageDialogService.DisplayAlertAsync("Bluetooth is off", "Please turn on Bluetooth", "Ok");
                            break;
                        case ManagerState.PoweredOn:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(state), state, null);
                    }
                });
        }

        public DelegateCommand DisplayPeripheralPageCommand { get; }
        public DelegateCommand DisplayCentralPageCommand { get; }

        private async Task DisplayPeripheralPage()
        {
            await _navigationService.NavigateAsync($"{nameof(PeripheralPage)}");
        }

        private async Task DisplayCentralPage()
        {
            await _navigationService.NavigateAsync($"{nameof(CentralPage)}");
        }
    }
}