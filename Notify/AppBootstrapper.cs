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
        private TaskbarIcon _notifyIcon;   
        public AppBootstrapper(TaskbarIcon notifyIcon)
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
            Updater = new DirectoryNameUpdater();

            FileService.InitializeConfigFile(ApplicationManager.Instance);

            if (notifyIcon != null)
            {
                if (ApplicationManager.Instance.MatchedAppInfos.Count > 0)
                {
                    foreach (var app in ApplicationManager.Instance.MatchedAppInfos)
                    {
                        if (app.CheckValidation())
                        {
                            app.Execute = () =>
                            {
                                try
                                {
                                    Process.Start(new ProcessStartInfo(app.ExecuteFile));
                                }
                                catch
                                {

                                }
                            };

                            var image = new Image
                            {
                                Source = FileToImageIconConverter.Icon(app.ExecuteFile)
                            };
                            var menuItem = new MenuItem
                            {
                                Header = app.ApplicationName,
                                ToolTip = app.ApplicationName,
                                Icon = image
                            };
                            menuItem.Click += (o, e) => app.Execute();
                            
                            if (notifyIcon.ContextMenu != null)
                            {
                                notifyIcon.ContextMenu.Items.Add(menuItem);
                            }
                        }
                    }
                }
            }
        }
        public IConfigurationViewModel ConfigViewModel { get; set; }
        public IComponentUpdater Updater { get; set; }
        
        public void Close()
        {
            FileService.SaveConfigurationFile(Resources.ConfigurationFile, ApplicationManager.Instance.RootPaths, ApplicationManager.Instance.ApplicationNames, ApplicationManager.Instance.MatchedAppInfos);
        }
    }
}