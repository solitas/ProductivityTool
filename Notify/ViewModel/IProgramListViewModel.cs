using ProductivityTool.Notify.Model;

using ReactiveUI;

using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IProgramListViewModel
    {
        bool IsAddDialogOpen { get; set; }
        ReactiveCommand<IExternalProgram, Unit> AddProgram { get; }
        ICommand RemoveProgram { get; }
        ICommand ModifyProgram { get; }
        ICommand Update { get; }
        IExternalProgram SelectedProgram { get; set; }

        IProgramInsertViewModel InsertViewModel { get; set; }

        ReadOnlyObservableCollection<IExternalProgram> Programs { get; }

    }
}