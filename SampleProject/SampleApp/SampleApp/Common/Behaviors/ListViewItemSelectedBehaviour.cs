using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SampleApp.Common.Behaviors
{
    public class ListViewItemSelectedBehaviour : Behavior<ListView>
    {
        private IDisposable _listViewSelectionDisposable;
        private IDisposable _listViewBindingContextChangedDisposable;
        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(nameof(ItemSelectedCommand), typeof(ICommand), typeof(ListViewItemSelectedBehaviour), null);

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand) GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value);}
        }

        protected override void OnAttachedTo(ListView bindable)
        {
            base.OnAttachedTo(bindable);
            _listViewSelectionDisposable?.Dispose();
            _listViewBindingContextChangedDisposable?.Dispose();
            _listViewBindingContextChangedDisposable = Observable.FromEventPattern(eh => bindable.BindingContextChanged += eh, eh => bindable.BindingContextChanged -= eh)
                .Subscribe(pattern =>
                {
                    BindingContext = bindable.BindingContext;
                });

            _listViewSelectionDisposable = Observable.FromEventPattern<EventHandler<SelectedItemChangedEventArgs>, SelectedItemChangedEventArgs>(eh => bindable.ItemSelected += eh, eh => bindable.ItemSelected -= eh)
                .Subscribe(eventPattern =>
                {
                    ItemSelectedCommand.Execute(eventPattern.EventArgs.SelectedItem);
                });
        }

        protected override void OnDetachingFrom(ListView bindable)
        {
            base.OnDetachingFrom(bindable);
            _listViewBindingContextChangedDisposable?.Dispose();
            _listViewSelectionDisposable?.Dispose();
        }
    }
}
