using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using SampleApp.Views;

namespace SampleApp.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        public MainPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            DisplayPeripheralPageCommand = DelegateCommand.FromAsyncHandler(DisplayPeripheralPage);
            DisplayCentralPageCommand = DelegateCommand.FromAsyncHandler(DisplayCentralPage);
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
