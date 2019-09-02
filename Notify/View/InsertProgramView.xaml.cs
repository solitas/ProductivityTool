using Microsoft.WindowsAPICodePack.Dialogs;

using ReactiveUI;
using System;
using System.IO;
using System.Windows;
using DynamicData;
using MessageBox = System.Windows.MessageBox;

namespace ProductivityTool.Notify.View
{
    /// <summary>
    /// InsertProgramView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InsertProgramView
    {
        public InsertProgramView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(this.BindCommand(ViewModel, viewModel => viewModel.InsertProgram, view => view.AcceptButton));
                d(this.Bind(ViewModel, viewModel => viewModel.ProgramLabel, view => view.InsertProgramLabel.Text));
                d(this.Bind(ViewModel, viewModel => viewModel.ExecutionProgramFile, view => view.InsertProgramFile.Text));
                d(this.Bind(ViewModel, viewModel => viewModel.RootDirectory, view => view.RootDirectory.Text));
                d(this.Bind(ViewModel, viewModel => viewModel.CopyToLocal, view => view.CopyToLocal.IsChecked));

                ViewModel.WhenAnyObservable(x => x.InsertProgram)
                    .Subscribe(p =>
                    {
                        if (p != null)
                            ApplicationManager.Instance.ExternalPrograms.AddOrUpdate(p);
                        DialogResult = true;
                    });
            });
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
            }
        }
    }
}
