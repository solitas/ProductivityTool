using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace TaskManagement.Core.ViewModel
{
    public interface ITaskInsertViewModel
    {
        ReactiveCommand<Unit, bool> Accept { get; set; }
        ICommand Cancel { get; set; }
    }
}