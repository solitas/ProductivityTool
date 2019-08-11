using DynamicData;
using ProductivityTool.Notify.Model;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace ProductivityTool.Notify.ViewModel
{
    public class NotifyIconViewModel : ReactiveObject, INotifyIconViewModel, IDisposable
    {
        private IDisposable _cleanup;
        public NotifyIconViewModel()
        {
            var configurationMenu = new ConfigurationMenu();
            var exitMenu = new ExitMenu();

            ApplicationManager.Instance.MatchedAppInfos.Add(configurationMenu);
            ApplicationManager.Instance.MatchedAppInfos.Add(exitMenu);

            var appManager = ApplicationManager.Instance;
            _cleanup = appManager.MatchedAppInfos
                .Connect()
                .Sort(new MatchedApplicationComparer(), SortOptions.UseBinarySearch)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var collection)
                .Subscribe();

            Applications = collection;
        }

        public ReadOnlyObservableCollection<MatchedApplication> Applications { get; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}
