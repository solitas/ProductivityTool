using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManagement.Core.ViewModel;

namespace TaskManagement.App.View
{
    public class UserTaskDetailViewBase : ReactiveUserControl<IDetailViewModel> { /* No code needed here */}
    /// <summary>
    /// UserTaskDetailView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserTaskDetailView : UserTaskDetailViewBase
    {
        public UserTaskDetailView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Tasks, v => v.DetailDataView.ItemsSource));
                d(this.BindCommand(ViewModel, vm => vm.Insert, v => v.InsertButton));
                d(this.BindCommand(ViewModel, vm => vm.Save, v => v.SaveButton));
            });
        }
    }
}
