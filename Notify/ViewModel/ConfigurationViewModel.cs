using DynamicData.Annotations;
using ProductivityTool.Notify.Model;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ProductivityTool.Notify.ViewModel
{
    public class ConfigurationViewModel : ReactiveObject, IConfigurationViewModel
    {
        private const string ConfigFile = "Configuration";
        private const int MaxCountOfRootPath = 3;
        private readonly IComponentUpdater _componentUpdater;
        
        private string _userInputAppName;

        public ApplicationManager Manager { get; set; }

        public string UserInputAppName
        {
            get => _userInputAppName;
            set => this.RaiseAndSetIfChanged(ref _userInputAppName, value);
        }
        
        private string _userInputRootPath;
        public string UserInputRootPath
        {
            get => _userInputRootPath;
            set => this.RaiseAndSetIfChanged(ref _userInputRootPath, value);
        }
        
        private string _userSelectAppName;
        public string UserSelectAppName
        {
            get => _userSelectAppName;
            set => this.RaiseAndSetIfChanged(ref _userSelectAppName, value);
        }
        
        private string _userSelectRootPath;
        public string UserSelectRootPath
        {
            get => _userSelectRootPath;
            set => this.RaiseAndSetIfChanged(ref _userSelectRootPath, value);
        }

        public ConfigurationViewModel(IComponentUpdater updater, ApplicationManager manager)
        {
            _componentUpdater = updater;
            Manager = manager;
           

            UpdateApp = ReactiveCommand.Create(UpdateApplications);
            ResetAppInfo = ReactiveCommand.Create(ResetApplication);

            var canAddApp = this.WhenAnyValue(x => x.UserInputAppName)
                                .Select(x => !string.IsNullOrEmpty(x) && FileNameFormatCheck(x));
            var canRemoveApp = this.WhenAnyValue(x => x.UserSelectAppName)
                .Select(x => !string.IsNullOrEmpty(x));

            var canAddRootPath = this.WhenAnyValue(x => x.UserInputRootPath)
                .Select(x => !string.IsNullOrEmpty(x));
            var canRemoveRootPath = this.WhenAnyValue(x => x.UserSelectRootPath)
                .Select(x => !string.IsNullOrEmpty(x));
           
            AddApplication = ReactiveCommand.Create(() =>
            {
                AddApp(UserInputAppName);
                UserInputAppName = string.Empty;
            }, canAddApp);
            
            RemoveApplication = ReactiveCommand.Create(() => { RemoveApp(UserSelectAppName);}, canRemoveApp);

            AddRootPath = ReactiveCommand.Create(() =>
            {
                if (Manager.RootPaths.All(name => name != UserInputRootPath))
                {
                    Manager.RootPaths.Add(UserInputRootPath);
                }

                UserInputRootPath = string.Empty;
            }, canAddRootPath);

            RemoveRootPath = ReactiveCommand.Create(() =>
            {
                if (Manager.RootPaths.Any(name => name == UserSelectRootPath))
                {
                    Manager.RootPaths.Remove(UserSelectRootPath);
                }
            }, canRemoveRootPath);

            SelectPath = ReactiveCommand.Create(() =>
            {
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

                    UserInputRootPath = dialog.FileName;
                }
            });
        }
//
//        public ObservableCollection<string> RootPaths => Manager.RootPaths;
//        public ObservableCollection<string> ApplicationNames => Manager.ApplicationNames;
//        public ObservableCollection<MatchedApplicationInfo> MatchedAppInfos => Manager.MatchedAppInfos;


        public ICommand UpdateApp { get; set; }
        public ICommand ResetAppInfo { get; set; }
        public ICommand AddApplication { get; set; }
        public ICommand AddRootPath { get; set; }
        public ICommand RemoveRootPath { get; set; }
        public ICommand RemoveApplication { get; set; }
        public ICommand SelectPath { get; set; }
        private async void UpdateApplications()
        {
            foreach (var root in Manager.RootPaths)
            {
                foreach (var appName in Manager.ApplicationNames)
                {
                    await FileService.SearchAsync(root, appName, _componentUpdater)
                        .ContinueWith(t =>
                        {
                            var file = t.Result;
                            var appInfo = GetExistsAppInfo(appName);
                            if (appInfo != null)
                            {
                                appInfo.File = file;
                            }
                            else
                            {
                                SetAppInfo(appName, file);
                            }
                        });
                }
            }
        }
        private void ResetApplication()
        {
            Manager.MatchedAppInfos.Clear();
        }
        private void AddApp(string appName)
        {
            if (Manager.ApplicationNames.All(name => name != appName))
            {
                Manager.ApplicationNames.Add(appName);
            }
        }
        private void RemoveApp(string appName)
        {
            if (Manager.ApplicationNames.Any(name => name == appName))
            {
                Manager.ApplicationNames.Remove(appName);
            }
        }
        private void SetAppInfo(string appName, string file)
        {
            if (Manager.MatchedAppInfos.All(appInfo => appInfo.ApplicationName != appName))
            {
                var newAppInfo = new MatchedApplicationInfo()
                {
                    ApplicationName = appName,
                    File = file
                };
                Manager.MatchedAppInfos.Add(newAppInfo);
            }
        }
        private MatchedApplicationInfo GetExistsAppInfo([NotNull]string appName)
        {
            return Manager.MatchedAppInfos.SingleOrDefault(x => x.ApplicationName == appName);
        }
        private bool FileNameFormatCheck(string appName)
        {
            return !Regex.IsMatch(appName, @"^(?!.*\.exe$).*$");
        }
    }
}
