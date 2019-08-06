using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using ProductivityTool.Notify.Model;
using ProductivityTool.Notify.Properties;
using ProductivityTool.Notify.ViewModel;
using ReactiveUI;
using Splat;

namespace ProductivityTool.Notify
{
    public class AppBootstrapper
    {
        public AppBootstrapper(TaskbarIcon notifyIcon)
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Updater = new DirectoryNameUpdater();

            FileService.InitializeConfigFile(ApplicationManager.Instance);
        }

        public IConfigurationViewModel ConfigViewModel { get; set; }
        public IComponentUpdater Updater { get; set; }
        
        public void Close()
        {
            FileService.SaveConfigurationFile(Resources.ConfigurationFile, ApplicationManager.Instance.RootPaths, ApplicationManager.Instance.ApplicationNames, ApplicationManager.Instance.MatchedAppInfos);
        }
    }
}