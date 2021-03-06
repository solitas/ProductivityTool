﻿using DynamicData;
using ProductivityTool.Notify.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
        private bool _isAddDialogOpen;

        public bool IsAddDialogOpen
        {
            get => _isAddDialogOpen;
            set => this.RaiseAndSetIfChanged(ref _isAddDialogOpen, value);
        }

        public ReactiveCommand<IExternalProgram, Unit> AddProgram { get; }
        public ICommand RemoveProgram { get; }
        public ICommand ModifyProgram { get; }
        public ICommand Update { get; }

        private IExternalProgram _selectedProgram;
        public IExternalProgram SelectedProgram
        {
            get => _selectedProgram;
            set => this.RaiseAndSetIfChanged(ref _selectedProgram, value);
        }

        private IProgramInsertViewModel _insertViewModel;

        public IProgramInsertViewModel InsertViewModel
        {
            get => _insertViewModel;
            set => this.RaiseAndSetIfChanged(ref _insertViewModel, value);
        }
        [Reactive]
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
                .Select(x => x != null)
                .Do(x =>
                {
                    Console.WriteLine("Is Selected");
                });

            AddProgram = ReactiveCommand.Create<IExternalProgram, Unit>(program =>
            {
                InsertViewModel = new ProgramInsertViewModel();

                Interactions.InsertProgramDialog.Handle(InsertViewModel).Subscribe();

                return Unit.Default;
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

            }, programSelected);
            Update = ReactiveCommand.Create(() => { });
        }

        public void Dispose()
        {
        }
    }
}
