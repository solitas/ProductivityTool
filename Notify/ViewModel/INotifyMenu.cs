using System.Windows.Controls;
using System.Windows.Input;

namespace ProductivityTool.Notify.ViewModel
{
    public interface INotifyMenu
    {
        ICommand Execute { get; set; }
        string Label { get; set; }
        Image IconImage { get; set; }
        int BadgeValue { get; set; }
    }
}