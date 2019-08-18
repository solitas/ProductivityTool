using DynamicData;

using ProductivityTool.Notify.Model;

using ReactiveUI;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ProductivityTool.Notify.ViewModel
{
    public class ProgramListViewModel : ReactiveObject, IProgramListViewModel, IDisposable
    {
        public Interaction<Unit, IExternalProgram> AddInteraction { get; } = new Interaction<Unit, IExternalProgram>();
        public Interaction<Unit, IExternalProgram> ModifyInteraction { get; } = new Interaction<Unit, IExternalProgram>();
        public ICommand AddProgram { get; }
        public ICommand RemoveProgram { get; }
        public ICommand ModifyProgram { get; }
        public ICommand Update { get; }
        public IExternalProgram SelectedProgram { get; set; }
        public ReadOnlyObservableCollection<IExternalProgram> Programs { get; }

        public ProgramListViewModel()
        {
            var manager = ApplicationManager.Instance;
            manager.ExternalPrograms.Connect()
                .Filter(app => app != null)
                .Bind(out var items)
                .Subscribe();

            Programs = items;

            var programSelected = this.WhenAnyValue(x => x.SelectedProgram)
                .Select(x => x != null);

            AddProgram = ReactiveCommand.Create(async () =>
            {
                var program = await AddInteraction.Handle(Unit.Default);
                if (program != null)
                {
                    manager.ExternalPrograms.AddOrUpdate(program);
                }
            });
            RemoveProgram = ReactiveCommand.Create(() =>
            {
                if (SelectedProgram != null)
                {
                    manager.ExternalPrograms.Remove(SelectedProgram);
                }
            }, programSelected);
            ModifyProgram = ReactiveCommand.Create(async () =>
            {
                if (SelectedProgram != null)
                {
                    await ModifyInteraction.Handle(Unit.Default);
                }
            }, programSelected);
            Update = ReactiveCommand.Create(() => { });
        }

        public void Dispose()
        {
        }
    }
}
