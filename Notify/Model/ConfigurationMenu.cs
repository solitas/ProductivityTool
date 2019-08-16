using System.Windows;
using ProductivityTool.Notify.View;
using ReactiveUI;

namespace ProductivityTool.Notify.Model
{
    public sealed class ConfigurationMenu : MatchedApplication
    {
        public ConfigurationMenu()
        {
            Header = "Configuration";
            Command = ReactiveCommand.Create(() =>
            {
                var configView = new ConfigurationWindow();
                configView.Show();
            });
        }
    }
}