using Microsoft.WindowsAPICodePack.Dialogs;
using ProductivityTool.Notify.Model;
using ProductivityTool.Notify.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ProductivityTool.Notify
{
    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : Window
    {
        private string _rootPath;
        private ExeAppContext _tempContext;
        private AppContextService _service;

        public ConfigurationView(AppContextService service)
        {
            InitializeComponent();
            _service = service;

            TaskbarIcon.ContextMenu = new ContextMenu();
            foreach (var context in _service.Contexts)
            {
                AddContext(context);
            }

            var configurationMenuItem = new MenuItem
            {
                Header = "Configuration"
            };

            configurationMenuItem.Click += (o, e) =>
            {
                if (!IsVisible)
                {
                    Visibility = Visibility.Visible;
                }
            };

            TaskbarIcon.ContextMenu.Items.Add(new Separator());
            TaskbarIcon.ContextMenu.Items.Add(configurationMenuItem);

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            AppContexts.ItemsSource = _service.Contexts;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            Visibility = Visibility.Hidden;
        }

        private void AddContext(ExeAppContext context)
        {
            var image = new Image
            {
                Source = FileToImageIconConverter.Icon(context.FullPath)
            };

            var menuItem = new MenuItem
            {
                Header = context.AppName,
                ToolTip = context.Description,
                Icon = image
            };

            menuItem.Click += (o, e) => context.Execute();
            TaskbarIcon.ContextMenu.Items.Add(menuItem);
        }
        private void Clear(object sender, RoutedEventArgs e)
        {
            RootPath.Text = string.Empty;
            ApplicationName.Text = string.Empty;
            FileInfoBox.Text = string.Empty;
            _tempContext = null;
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            _tempContext = null;

            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false
            };

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName;
                var info = new DirectoryInfo(path);

                if ((info.Attributes & FileAttributes.Directory) == 0)
                {
                    MessageBox.Show("this is not directory");
                    return;
                }

                _rootPath = dialog.FileName;
                RootPath.Text = dialog.FileName;
            }
        }

        private async void CheckButtonClicked(object sender, RoutedEventArgs e)
        {
            var fileName = ApplicationName.Text;

            if (string.IsNullOrEmpty(_rootPath))
            {
                MessageBox.Show("Root Path cannot be empty");
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Application Name cannot be empty");
                return;
            }

            if (!Directory.Exists(_rootPath))
            {
                MessageBox.Show("Root Path is not exists");
                return;
            }
            CheckButton.IsEnabled = false;
            DirectoryNameUpdater updater = new DirectoryNameUpdater
            {
                InvokeUpdate = (message) =>
                {
                    Dispatcher.BeginInvoke(new Action(() => { FileInfoBox.Text = message; }));
                }
            };
            await FileService.SearchAsync(_rootPath, fileName, updater).ContinueWith(t =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!string.IsNullOrEmpty(t.Result))
                    {
                        _tempContext = new ExeAppContext(t.Result);
                        FileInfoBox.Text = _tempContext.ToString();
                    }
                    CheckButton.IsEnabled = true;
                }));
            });



            //Task.Run(() =>
            //{
            //    var maxTime = DateTime.MinValue;
            //    var maxApp = string.Empty;
            //    var maxApp2 = string.Empty;

            //    var dirs = Directory.GetDirectories(_rootPath);

            //    var maxVersion = 0;

            //    foreach (var dir in dirs)
            //    {
            //        var files = Directory.GetFiles(dir, "*.exe", SearchOption.AllDirectories);
            //        var targetExecuteFiles = files.Where(x => x.Contains(fileName));

            //        foreach (var targetFile in targetExecuteFiles)
            //        {
            //            var info = new FileInfo(targetFile);

            //            var versionInfo = FileVersionInfo.GetVersionInfo(targetFile);
            //            var version = versionInfo.ProductBuildPart + versionInfo.ProductMajorPart +
            //                          versionInfo.ProductMinorPart + versionInfo.ProductPrivatePart;
            //            if (maxVersion < version)
            //            {
            //                maxVersion = version;
            //                maxApp2 = targetFile;
            //            }
            //            if (maxTime < info.LastWriteTime)
            //            {
            //                maxTime = info.LastWriteTime;
            //                maxApp = targetFile;
            //            }
            //        }
            //    }

            //    var targetApp = maxApp2 == maxApp ? maxApp2 : maxApp;

            //    if (!string.IsNullOrEmpty(targetApp))
            //    {
            //        Dispatcher.BeginInvoke(new Action(() =>
            //        {
            //            _tempContext = new ExeAppContext(targetApp);
            //            FileInfoBox.Text = _tempContext.ToString();
            //        }));
            //    }
            //});
        }

        private void AddContext(object sender, RoutedEventArgs e)
        {
            if (_tempContext != null)
            {
                _service.EnrollApplication(_tempContext);
                AddContext(_tempContext);
                _tempContext = null;
            }
        }
    }
}
