using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SampleApp.Common.Behaviors
{
    public interface IPageAppearingAware
    {
        void OnAppearing(Page page);
        void OnDisappearing(Page page);
    }

    public class PageAppearingBehavior : Behavior<Page>
    {
        private IDisposable _pageDisappearingDisposable;
        private IDisposable _pageAppearingDisposable;

        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
            _pageAppearingDisposable?.Dispose();
            _pageDisappearingDisposable?.Dispose();

            _pageAppearingDisposable = Observable.FromEventPattern(eh => bindable.Appearing += eh,
                eh => bindable.Appearing -= eh).Subscribe(_ =>
                {
                    IPageAppearingAware pageAppearingAware = bindable.BindingContext as IPageAppearingAware;
                    pageAppearingAware?.OnAppearing(bindable);
                });
            _pageDisappearingDisposable = Observable.FromEventPattern(eh => bindable.Disappearing += eh,
                eh => bindable.Disappearing -= eh).Subscribe(_ =>
                {
                    IPageAppearingAware pageAppearingAware = bindable.BindingContext as IPageAppearingAware;
                    pageAppearingAware?.OnDisappearing(bindable);
                });

        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
            _pageAppearingDisposable?.Dispose();
            _pageDisappearingDisposable?.Dispose();
        }
    }

    public interface INavigationBackAware
    {
        void PagePopped();
    }

    public class NavigationBackAwareBehavior : Behavior<NavigationPage>
    {
        private IDisposable _eventDisposable;
        protected override void OnAttachedTo(NavigationPage bindable)
        {
            base.OnAttachedTo(bindable);
            _eventDisposable =  Observable.FromEventPattern<EventHandler<NavigationEventArgs>, NavigationEventArgs>(eh => bindable.Popped += eh, eh => bindable.Popped -= eh).Subscribe(
                pattern =>
                {
                    INavigationBackAware navigationBackAware = pattern.EventArgs.Page.BindingContext as INavigationBackAware;;
                    navigationBackAware?.PagePopped();
                });
        }

        protected override void OnDetachingFrom(NavigationPage bindable)
        {
            base.OnDetachingFrom(bindable);
            _eventDisposable.Dispose();
        }
    }
}
