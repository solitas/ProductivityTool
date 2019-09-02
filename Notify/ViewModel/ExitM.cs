using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;

namespace ProductivityTool.Notify.ViewModel
{
    public class ExitM : ReactiveObject, INotifyMenu
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
        public ExitM()
        {
            Label = "Exit";
            Execute = ReactiveCommand.Create(() =>
            {
                Application.Current.Shutdown();
            });
        }
    }
}