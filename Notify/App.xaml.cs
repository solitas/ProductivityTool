﻿using Hardcodet.Wpf.TaskbarNotification;
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
            var dispatcher = Dispatcher;

            Interactions.QuestionUpdateApplication.RegisterHandlerForDialog(context =>
            {
                var message = context.Input;
                dispatcher.Invoke(() =>
                {
                    var configView = new QuestionWindow();
                    configView.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    var result = configView.ShowDialog();
                    context.SetOutput(result == true);
                });
//                var result = MessageBox.Show(message, "question", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
//                context.SetOutput(result == MessageBoxResult.Yes);
            });
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _bootstrapper.Close();
        }
    }
}
