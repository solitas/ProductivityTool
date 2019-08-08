using System.Windows;
using ProductivityTool.Notify.View;
using ReactiveUI;

namespace ProductivityTool.Notify.Model
{
    public class ConfigurationMenu : MatchedApplication
    {
        public ConfigurationMenu()
        {
            Command = ReactiveCommand.Create(() =>
            {
                var configView = new ConfigView { WindowStartupLocation = WindowStartupLocation.CenterScreen };
                configView.Show();
            });
        }
    }
}