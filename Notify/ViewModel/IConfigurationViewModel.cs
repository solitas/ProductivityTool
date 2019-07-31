using ProductivityTool.Notify.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IConfigurationViewModel : IReactiveObject
    {
        ApplicationManager Manager { get; set; }

        string UserInputAppName { get; set; }
        string UserInputRootPath { get; set; }
        string UserSelectAppName { get; set; }
        string UserSelectRootPath { get; set; }

        ICommand UpdateApp { get; set; }
        ICommand ResetAppInfo { get; set; }
        ICommand AddApplication { get; set; }
        ICommand RemoveApplication { get; set; }
        ICommand AddRootPath { get; set; }
        ICommand RemoveRootPath { get; set; }
        ICommand SelectPath { get; set; }
    }
}
