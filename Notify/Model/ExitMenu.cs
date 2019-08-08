using System.Windows;
using ReactiveUI;

namespace ProductivityTool.Notify.Model
{
    public class ExitMenu : MatchedApplication
    {
        public ExitMenu()
        {
            Command = ReactiveCommand.Create(() =>
            {
                Application.Current.Shutdown();
            });
        }
    }
}