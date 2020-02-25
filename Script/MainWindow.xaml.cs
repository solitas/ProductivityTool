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

namespace Script
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : IViewFor<MainViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty
            = DependencyProperty.Register(
                "ViewModel", typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(null));


        public MainWindow()
        {
            InitializeComponent();
            this.ViewModel = new MainViewModel();
            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.Document, v=>v.Code.Document);
                this.Bind(ViewModel, vm => vm.Output, v => v.Output.Text);
                this.BindCommand(ViewModel, vm => vm.Test, v => v.Test);
            });
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }

        public MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => this.SetValue(ViewModelProperty, value);
        }
    }
}
