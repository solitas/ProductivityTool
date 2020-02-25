using System;
using System.Windows;

namespace TaskManagement.App
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var monitors = Monitor.AllMonitors;
            Monitor primaryMonitor = null;
            foreach (var monitor in monitors)
            {
                Console.WriteLine(@"Monitor
1.Bounds={0}
2.IsPrimary={1}
3.Name={2}
4.workingArea={3}
",
                    monitor.Bounds, monitor.IsPrimary, monitor.Name, monitor.WorkingArea);
                if (monitor.IsPrimary)
                {
                    primaryMonitor = monitor;
                }
            }

            var mainWindow = new MainWindow();
            mainWindow.Show();
            
            // adjust the window size
            mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            var mainWindowPresentationSource = PresentationSource.FromVisual(mainWindow);
            
            double f1 = .8f, f2 = .8f;

            if (mainWindowPresentationSource?.CompositionTarget != null)
            {
                var m = mainWindowPresentationSource.CompositionTarget.TransformFromDevice;
                f1 = m.M11;
                f2 = m.M22;
            }

            if (primaryMonitor != null)
            {
                var compositionWidth = primaryMonitor.WorkingArea.Width * 0.8 * f1;
                var compositionHeight = primaryMonitor.WorkingArea.Height * 0.65 * f2;
                
                if (compositionWidth > 1920) compositionWidth = 1920 * 0.8;
                if (compositionHeight > 1080) compositionHeight = 1080 * 0.8;

                mainWindow.Width = compositionWidth;
                mainWindow.Height = compositionHeight;
            }
        }                                                                                                                                                                                                                                                                                                                                                                  
    }
}
