using ProductivityTool.Notify.View;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProductivityTool.Notify.ViewModel
{

    public interface INotifyIconViewModel
    {

    }

    public class NotifyIconViewModel : ReactiveObject
    {

        public NotifyIconViewModel()
        {
          
            Configuration = ReactiveCommand.Create(() =>
            {
                var configView = new ConfigView
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                configView.Show();
            });

            ExitApplication = ReactiveCommand.Create(() =>
            {
                Application.Current.Shutdown();
            });

        }


        public ICommand ExitApplication { get; set; }
        public ICommand Configuration { get; set; }
    }
}
