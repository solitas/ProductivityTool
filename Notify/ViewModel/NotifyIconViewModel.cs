using ProductivityTool.Notify.View;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using DynamicData;
using DynamicData.Annotations;
using DynamicData.Binding;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify.ViewModel
{
    public class NotifyIconViewModel : ReactiveObject, INotifyIconViewModel, IDisposable
    {
        public NotifyIconViewModel()
        {

            var configurationMenu = new ConfigurationMenu()
            {
                Header = "Configuration"
            };


            var exitMenu = new ExitMenu()
            {
                Header = "Exit"
            };

            ApplicationManager.Instance.MatchedAppInfos.Add(configurationMenu);
            ApplicationManager.Instance.MatchedAppInfos.Add(exitMenu);
            var instance = ApplicationManager.Instance;

            ApplicationManager.Instance.MatchedAppInfos
                .Connect()
                .Sort(new MatchedApplicationComparer(), SortOptions.UseBinarySearch)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out var data)
                .Subscribe();

            Applications = data;
        }

        public ReadOnlyObservableCollection<MatchedApplication> Applications { get; }

        public void Dispose()
        {

        }
    }
}
