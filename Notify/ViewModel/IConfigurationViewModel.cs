using ProductivityTool.Notify.Model;

using ReactiveUI;

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IConfigurationViewModel : IReactiveObject
    {
        ApplicationManager Manager { get; set; }
        string SelectedRootPath { get; set; }
        ApplicationModel SelectedAppModel { get; set; }
        MatchedApplication SelectedMatchedApplication { get; set; }
        ReadOnlyObservableCollection<MatchedApplication> MatchedItems { get; }

        ICommand UpdateApp { get; set; }
        ICommand ResetAppInfo { get; set; }
        ICommand AddApplication { get; set; }
        ICommand RemoveApplication { get; set; }
        ICommand AddRootPath { get; set; }
        ICommand RemoveRootPath { get; set; }
    }
}
