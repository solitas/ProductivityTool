using ProductivityTool.Notify.ViewModel;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace ProductivityTool.Notify.View
{
    /// <summary>
    /// ProgramListView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgramListView
    {
        public ProgramListView()
        {
            InitializeComponent();
            ViewModel = new ProgramListViewModel();

            this.WhenActivated(d =>
            {
                d(this.Bind(ViewModel, viewModel => viewModel.SelectedProgram, view => view.Programs.SelectedItem));
                d(this.OneWayBind(ViewModel, viewModel => viewModel.Programs, view => view.Programs.ItemsSource));
                d(this.BindCommand(ViewModel, viewModel => viewModel.AddProgram, view => view.AddButton));
                d(this.BindCommand(ViewModel, viewModel => viewModel.RemoveProgram, view => view.RemoveButton));
                d(this.BindCommand(ViewModel, viewModel => viewModel.ModifyProgram, view => view.ModifyButton));

                d(Interactions.InsertProgramDialog.RegisterHandler(handler =>
                {
                    var viewModel = handler.Input;
                    var view = new InsertProgramView { ViewModel = viewModel };
                    var result = view.ShowDialog();

                    handler.SetOutput(result == true);
                }));
            });
        }
    }
}
