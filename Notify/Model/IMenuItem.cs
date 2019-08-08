using System.Windows.Controls;
using System.Windows.Input;
using ProductivityTool.Notify.ViewModel;

namespace ProductivityTool.Notify.Model
{
    public interface IMenuItem
    {
        string Header { get; set; }
        Image Icon { get; set; }
        ICommand Command { get; set; }
    }
}