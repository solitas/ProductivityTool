using DynamicData;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

using TaskManagement.Core.Model;

namespace TaskManagement.Core.ViewModel
{
    public class MasterViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<UserTask> _tasks;
        private readonly ReadOnlyObservableCollection<UserTask> _urgentImportantTasks;
        private readonly ReadOnlyObservableCollection<UserTask> _urgentLessImportantTasks;
        private readonly ReadOnlyObservableCollection<UserTask> _lessUrgentImportantTasks;
        private readonly ReadOnlyObservableCollection<UserTask> _lessUrgentLessImportantTasks;

        public MasterViewModel(Action shutDownApp)
        {
            List<UserTask> list = null;
            try
            {
                list = FileService.Load();
            }
            catch
            {
                // ignored
            }

            var sources = new SourceList<UserTask>();
            
            if (list != null)
            {
                sources.AddRange(list);
            }
            sources.Connect()
                .Bind(out _tasks)
                .Subscribe();

            sources.Connect()
                .Filter(task => task.Important == ImportantLevel.High)
                .Filter(task => task.Urgent == UrgentLevel.High)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _urgentImportantTasks)
                .Subscribe();

            sources.Connect()
                .Filter(task => task.Important == ImportantLevel.Low)
                .Filter(task => task.Urgent == UrgentLevel.High)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _urgentLessImportantTasks)
                .Subscribe();

            sources.Connect()
                .Filter(task => task.Important == ImportantLevel.High)
                .Filter(task => task.Urgent == UrgentLevel.Low)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _lessUrgentImportantTasks)
                .Subscribe();

            sources.Connect()
                .Filter(task => task.Important == ImportantLevel.Low)
                .Filter(task => task.Urgent == UrgentLevel.Low)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _lessUrgentLessImportantTasks)
                .Subscribe();

            InsertNewTask = new Interaction<ITaskInsertViewModel, Unit>();

            Insert = ReactiveCommand.Create(async () =>
            {
                await InsertNewTask.Handle(new TaskInsertViewModel(task =>
                {
                    sources.Edit(x =>
                    {
                        x.Add(task);
                    });
                }));
            });

            Delete = ReactiveCommand.Create(() =>
            {
                if (SelectedTask == null) return;
                sources.Edit(x => { x.Remove(SelectedTask); });
            });

            Modify = ReactiveCommand.Create(async () =>
            {
                if (SelectedTask == null) return;
                await InsertNewTask.Handle(new TaskInsertViewModel(SelectedTask));
            });

            Save = ReactiveCommand.Create(() => FileService.Save(sources.Items.ToList()));

            Exit = ReactiveCommand.Create(() =>
            {
                Save.Execute(null);
                shutDownApp();
            });
        }

        public ReadOnlyObservableCollection<UserTask> Tasks => _tasks;
        public ReadOnlyObservableCollection<UserTask> UrgentImportantTasks => _urgentImportantTasks;
        public ReadOnlyObservableCollection<UserTask> UrgentLessImportantTasks => _urgentLessImportantTasks;
        public ReadOnlyObservableCollection<UserTask> LessUrgentImportantTasks => _lessUrgentImportantTasks;
        public ReadOnlyObservableCollection<UserTask> LessUrgentLessImportantTasks => _lessUrgentLessImportantTasks;
        public ICommand Insert { get; }
        public ICommand Save { get; }
        public ICommand Delete { get; }
        public ICommand Modify { get; }
        public ICommand Exit { get; }

        public Interaction<ITaskInsertViewModel, Unit> InsertNewTask { get; }

        [Reactive]
        public UserTask SelectedTask { get; set; }
    }
}
