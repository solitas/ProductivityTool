using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;
using ProductivityTool.Notify.View;

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

            Interactions.QuestionUpdateApplication.RegisterHandlerForDialog(context =>
            {
                var name = context.Input;
                var configView = new QuestionWindow(name)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                var result = configView.ShowDialog();
                context.SetOutput(result == true);
            });
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _bootstrapper.Close();
        }
    }
}
