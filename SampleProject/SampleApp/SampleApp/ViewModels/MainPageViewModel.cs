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
        }

        public DelegateCommand DisplayPeripheralPageCommand { get; }

        private async Task DisplayPeripheralPage()
        {
            await _navigationService.NavigateAsync($"{nameof(PeripheralPage)}");
        }
    }
}
