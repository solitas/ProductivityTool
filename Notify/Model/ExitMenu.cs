using System.Windows;
using ReactiveUI;

namespace ProductivityTool.Notify.Model
{
    public sealed class ExitMenu : MatchedApplication
    {
        public ExitMenu()
        {
            Header = "Exit";
            Command = ReactiveCommand.Create(() =>
            {
                Application.Current.Shutdown();
            });
        }
    }
}