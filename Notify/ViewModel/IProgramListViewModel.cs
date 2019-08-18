using System.Collections.ObjectModel;
using System.Windows.Input;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IProgramListViewModel
    {
        ICommand AddProgram { get; }
        ICommand RemoveProgram { get; }
        ICommand ModifyProgram { get; }
        ICommand Update { get; }
        IExternalProgram SelectedProgram { get; set; }
        ReadOnlyObservableCollection<IExternalProgram> Programs { get; }
    }
}