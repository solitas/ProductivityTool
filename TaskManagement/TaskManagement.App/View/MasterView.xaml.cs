using ReactiveUI;

using System;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using TaskManagement.Core.Model;
using TaskManagement.Core.ViewModel;

namespace TaskManagement.App.View
{
    public abstract class MasterViewBase : ReactiveUserControl<MasterViewModel>
    {

    }
    /// <summary>
    /// MasterView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MasterView
    {
        public MasterView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
            this.WhenActivated(d =>
            {
                d(this.ViewModel.InsertNewTask.RegisterHandler(
                    interaction =>
                    {
                        var view = new TaskInsertView
                        {
                            ViewModel = interaction.Input, 
                            Owner = Window.GetWindow(this),
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        view.ShowDialog();
                        interaction.SetOutput(Unit.Default);
                    }));

                d(ListItemSelectedHandle(List1));
                d(ListItemSelectedHandle(List2));
                d(ListItemSelectedHandle(List3));
                d(ListItemSelectedHandle(List4));
            });
        }

        private IDisposable ListItemSelectedHandle(ListBox list)
        {
            return list.Events().PreviewMouseDown.Subscribe(x =>
            {
                var item = list.SelectedItem as UserTask;
                ViewModel.SelectedTask = item;
            });
        }
    }
}
