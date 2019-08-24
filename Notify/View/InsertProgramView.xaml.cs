using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using ProductivityTool.Notify.ViewModel;
using ReactiveUI;
using MessageBox = System.Windows.MessageBox;

namespace ProductivityTool.Notify.View
{
    public abstract class InsertProgramViewBase : ReactiveUserControl<IProgramInsertViewModel>
    {

    }
    /// <summary>
    /// InsertProgramView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InsertProgramView
    {
        public InsertProgramView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, view => view.DataContext);
        }

        private void DirPath_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            var result = dialog.ShowDialog();
            if (result != CommonFileDialogResult.Ok) return;

            var path = dialog.FileName;
            var info = new DirectoryInfo(path);

            if ((info.Attributes & FileAttributes.Directory) == 0)
            {
                MessageBox.Show("this is not directory");
            }
            else
            {
                RootDirectory.Text = dialog.FileName;
                ViewModel.RootDirectory = dialog.FileName;
            }
        }
    }
}
