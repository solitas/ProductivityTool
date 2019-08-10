using ProductivityTool.Notify.ViewModel;

using ReactiveUI;

using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ProductivityTool.Notify.View
{
    public abstract class ConfigurationViewBase : ReactiveUserControl<IConfigurationViewModel>
    {

    }
    /// <summary>
    /// ConfigView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConfigView
    {
        public ConfigView()
        {
            InitializeComponent();
            var updater = new DirectoryNameUpdater();
            Action<string> directoryUpdateAction = (message) =>
           {
               DirectoryLog.Text = message;
           };
            updater.DirectoryFieldUpdated = message =>
            {
                Dispatcher.BeginInvoke(directoryUpdateAction, message);
            };
            ViewModel = new ConfigurationViewModel(updater, ApplicationManager.Instance);

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, viewModel => viewModel.Manager.RootPaths, view => view.RootPathItems.ItemsSource);
                this.OneWayBind(ViewModel, viewModel => viewModel.Manager.ApplicationModels, view => view.AppNameItems.ItemsSource);
                this.OneWayBind(ViewModel, viewModel => viewModel.MatchedItems, view => view.MatchedAppItems.ItemsSource);
                this.Bind(ViewModel, viewModel => viewModel.SelectedAppModel, view => view.AppNameItems.SelectedItem);
                this.BindCommand(ViewModel, viewModel => viewModel.AddRootPath, view => view.AddRootPath);
                this.BindCommand(ViewModel, viewModel => viewModel.AddApplication, view => view.AddApplication);
                this.BindCommand(ViewModel, viewModel => viewModel.UpdateApp, view => view.UpdateButton);
                

                d(Interactions.RootPathSelect.RegisterHandler(context =>
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
                        context.SetOutput(string.Empty); 
                    }
                    else
                    {
                        context.SetOutput(dialog.FileName);
                    }
                }));
                d(Interactions.ApplicationFileSelect.RegisterHandler(context =>
                {
                    var view = new ApplicationFileSelectView();
                    context.SetOutput(view.ShowDialog() == true ? view.ApplicationFile : string.Empty);
                }));
            });
        }
    }
}
