using DynamicData;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using TaskManagement.Core;
using TaskManagement.Core.Model;
using TaskManagement.Core.ViewModel;

namespace TaskManagement.App.ViewModel
{
    public class DetailViewModel : ReactiveObject, IDetailViewModel
    {
        private readonly SourceList<UserTask> _sources;
        private readonly ReadOnlyObservableCollection<UserTask> _items;
        public DetailViewModel()
        {
            _sources = new SourceList<UserTask>();
            _sources.Connect()
                    .Bind(out _items)
                    .Subscribe();

            _sources.AddRange(GetDemoTask());

            Save = ReactiveCommand.Create(() =>
            {
                FileService.Save(_sources.Items.ToList());
            });

            Insert = ReactiveCommand.Create(()=>
            {
                _sources.Edit(x =>
                {
                    x.Add(new UserTask() {
                        Contents=$"{DateTime.Now.ToString("hh:mm:ss")}",
                    });
                });
            });
        }
        public ReadOnlyObservableCollection<UserTask> Tasks => _items;
        public ICommand Insert { get; set; }
        public ICommand Remove { get; set; }
        public ICommand Modify { get; set; }
        public ICommand Save { get; set; }
        private IEnumerable<UserTask> GetDemoTask()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new UserTask()
                {
                    Contents = $@"Task-{i}",
                    Urgent = Core.UrgentLevel.High,
                    Important =  Core.ImportantLevel.High
                };
            }
        }
    }
}
