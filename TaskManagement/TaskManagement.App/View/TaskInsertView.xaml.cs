using ReactiveUI;

using System;

using TaskManagement.Core.ViewModel;

namespace TaskManagement.App.View
{
    /// <summary>
    /// TaskInsertView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TaskInsertView
    {
        public TaskInsertView()
        {
            InitializeComponent();
            this.WhenAnyValue(v => v.ViewModel).BindTo(this, v => v.DataContext);
            this.WhenActivated(d =>
            {
                this.ViewModel.WhenAnyObservable(x => x.Accept)
                    .Subscribe(x => { DialogResult = true; });
            });
        }
    }
}
