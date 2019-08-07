using ProductivityTool.Notify.View;
using ReactiveUI;
using System;
using System.Collections.Generic;
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
using DynamicData.Binding;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify.ViewModel
{

    public interface INotifyIconViewModel
    {

    }

    public class NotifyIconViewModel : ReactiveObject
    {
        public ObservableCollection<ContextMenuItemViewModel> Menus { get; set; }

        public NotifyIconViewModel()
        {
            Menus = new ObservableCollection<ContextMenuItemViewModel>();
            var configurationMenu = new ContextMenuItemViewModel(() =>
            {
                var configView = new ConfigView { WindowStartupLocation = WindowStartupLocation.CenterScreen };
                configView.Show();
            })
            { Header = "Configuration" };


            var exitMenu = new ContextMenuItemViewModel(() => { Application.Current.Shutdown(); })
            { Header = "Exit" };
            var sp = new ContextMenuItemViewModel(null)
            {
                Header = "-"
            };
            Menus.Add(sp);
            Menus.Add(configurationMenu);
            Menus.Add(exitMenu);

            var instance = ApplicationManager.Instance;
            var observable = instance.MatchedAppInfos
                .ToObservableChangeSet()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ActOnEveryObject(
                    addedItem =>
                    {
                        var appName = System.IO.Path.GetFileNameWithoutExtension(addedItem.ExecuteFile);
                        var menu = new ContextMenuItemViewModel(() =>
                        {
                            try
                            {
                                Process.Start(new ProcessStartInfo(addedItem.ExecuteFile));
                            }
                            catch
                            {
                            }
                        })
                        {
                            Header = appName
                        };

                        menu.SetIcon(addedItem.ExecuteFile);
                        Menus.Add(menu);

                        var configIndex = Menus.IndexOf(configurationMenu);
                        var exitIndex = Menus.IndexOf(configurationMenu);
                        var spIndex = Menus.IndexOf(sp);
                        Menus.Move(spIndex, Menus.Count - 1);
                        Menus.Move(configIndex, Menus.Count - 1);
                        Menus.Move(exitIndex, Menus.Count - 1);
                    },
                    removeItem =>
                        {
                            Console.WriteLine(removeItem.ExecuteFile);
                        });
        }

        public SourceList<MatchedApplicationInfo> MatchedInfo { get; set; }
    }
}
