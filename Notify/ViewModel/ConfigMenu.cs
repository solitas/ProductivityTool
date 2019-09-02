using System.Windows.Controls;
using System.Windows.Input;
using ProductivityTool.Notify.View;
using ReactiveUI;

namespace ProductivityTool.Notify.ViewModel
{
    public class ConfigMenu : ReactiveObject, INotifyMenu
    {
        public ICommand Execute { get; set; }
        public string Label { get; set; }
        public Image IconImage { get; set; }
        private int _badgeValue;
        public int BadgeValue
        {
            get => _badgeValue;
            set => this.RaiseAndSetIfChanged(ref _badgeValue, value);
        }

        public ConfigMenu()
        {
            Label = "Configuration";
            Execute = ReactiveCommand.Create(() =>
            {
                var programListWindow = new ProgramListWindow();
                programListWindow.Show();
            });
        }
    }
}