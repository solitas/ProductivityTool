using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace ProductivityTool.Notify
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private AppBootstrapper _bootstrapper;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            _bootstrapper = new AppBootstrapper(notifyIcon);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _bootstrapper.Close();
        }
    }
}
