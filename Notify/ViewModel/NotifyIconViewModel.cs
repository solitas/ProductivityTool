using DynamicData;
using ProductivityTool.Notify.Model;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Reactive.Linq;

namespace ProductivityTool.Notify.ViewModel
{
    public class NotifyIconViewModel : ReactiveObject, INotifyIconViewModel, IDisposable
    {
        private IDisposable _cleanup;

        public NotifyIconViewModel()
        {
            var appManager = ApplicationManager.Instance;
            _cleanup = appManager.Menus.Connect()
                .Sort(new MenuComparer())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var menus)
                .Subscribe();

            MenuItems = menus;
        }
        public ReadOnlyObservableCollection<INotifyMenu> MenuItems { get; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}
