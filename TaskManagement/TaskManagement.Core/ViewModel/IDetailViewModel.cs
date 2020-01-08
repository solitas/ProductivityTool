using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManagement.Core.Model;

namespace TaskManagement.Core.ViewModel
{
    public interface IDetailViewModel
    {
        ReadOnlyObservableCollection<UserTask> Tasks { get; }
        ICommand Insert { get; set; }
        ICommand Remove { get; set; }
        ICommand Modify { get; set; }
        ICommand Save { get; set; }
    }
}
