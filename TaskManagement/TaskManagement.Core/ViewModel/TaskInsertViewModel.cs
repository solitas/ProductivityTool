using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System.Windows.Input;

using TaskManagement.Core.Model;

namespace TaskManagement.Core.ViewModel
{
    /// <summary>
    /// Task insert callback function
    /// </summary>
    /// <param name="task">result task</param>
    public delegate void TaskInsertCallback(UserTask task);
    public class TaskInsertViewModel : ReactiveObject, ITaskInsertViewModel
    {
        public TaskInsertViewModel()
        {
            Task = new UserTask { Contents = "", Important = ImportantLevel.High, Urgent = UrgentLevel.High };
        }

        public TaskInsertViewModel(TaskInsertCallback callbackFunction) : this()
        {
            Accept = ReactiveCommand.Create<Unit,bool>(_ =>
            {
                if (!string.IsNullOrEmpty(Task.Contents))
                {
                    callbackFunction(Task);
                }

                return true;
            });
        }

        public TaskInsertViewModel(UserTask task) : this()
        {
            Task.Contents = task.Contents;
            Task.Urgent = task.Urgent;
            Task.Important = task.Important;

            Accept = ReactiveCommand.Create<Unit, bool>(_ =>
            {
                if (!string.IsNullOrEmpty(Task.Contents))
                {
                    task.Contents = Task.Contents;
                    task.Urgent = Task.Urgent;
                    task.Important = Task.Important;
                }

                return true;
            });
        }

        [Reactive]
        public UserTask Task { get; set; }

        public ReactiveCommand<Unit, bool> Accept { get; set; }
        public ICommand Cancel { get; set; }
    }
}