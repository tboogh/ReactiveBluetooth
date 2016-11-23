using Prism.Navigation;
using Xamarin.Forms;

namespace Demo.Views
{
    public partial class MainNavigationPage : NavigationPage
    {

        public MainNavigationPage()
        {
            InitializeComponent();
            Popped += OnPopped;
        }

        private void OnPopped(object sender, NavigationEventArgs navigationEventArgs)
        {
            var aware = (INavigationAware)navigationEventArgs.Page.BindingContext;
            aware?.OnNavigatedFrom(new NavigationParameters());
        }
    }
}
