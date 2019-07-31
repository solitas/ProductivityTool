using System.Collections.Generic;
using DynamicData.Annotations;
using ProductivityTool.Notify.Model;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DynamicData;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace ProductivityTool.Notify.ViewModel
{
    public class ConfigurationViewModel : ReactiveObject, IConfigurationViewModel
    {
        private const int MaxCountOfRootPath = 3;

        private readonly IComponentUpdater _componentUpdater;

        private string _userInputAppName;
        private string _userInputRootPath;
        private string _userSelectAppName;
        private string _userSelectRootPath;

        public ConfigurationViewModel(IComponentUpdater updater, ApplicationManager manager)
        {
            _componentUpdater = updater;
            Manager = manager;


            UpdateApp = ReactiveCommand.Create(async () =>
            {
                var result = await UpdateApplications();
                manager.MatchedAppInfos.AddRange(result);
            });
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

            RemoveApplication = ReactiveCommand.Create(() => { RemoveApp(UserSelectAppName); }, canRemoveApp);

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

        public ApplicationManager Manager { get; set; }
        public string UserInputAppName
        {
            get => _userInputAppName;
            set => this.RaiseAndSetIfChanged(ref _userInputAppName, value);
        }
        public string UserInputRootPath
        {
            get => _userInputRootPath;
            set => this.RaiseAndSetIfChanged(ref _userInputRootPath, value);
        }
        public string UserSelectAppName
        {
            get => _userSelectAppName;
            set => this.RaiseAndSetIfChanged(ref _userSelectAppName, value);
        }
        public string UserSelectRootPath
        {
            get => _userSelectRootPath;
            set => this.RaiseAndSetIfChanged(ref _userSelectRootPath, value);
        }
        public ICommand UpdateApp { get; set; }
        public ICommand ResetAppInfo { get; set; }
        public ICommand AddApplication { get; set; }
        public ICommand AddRootPath { get; set; }
        public ICommand RemoveRootPath { get; set; }
        public ICommand RemoveApplication { get; set; }
        public ICommand SelectPath { get; set; }

        private async Task<List<MatchedApplicationInfo>> UpdateApplications()
        {
            var result = new List<MatchedApplicationInfo>();
            
            ResetApplication();

            foreach (var root in Manager.RootPaths)
            {
                foreach (var appName in Manager.ApplicationNames)
                {
                    await FileService.SearchAsync(root, appName, _componentUpdater)
                        .ContinueWith(t =>
                        {
                            var file = t.Result;
                            if (!string.IsNullOrEmpty(file))
                            {
                                var newAppInfo = SetAppInfo(appName, file);
                                result.Add(newAppInfo);
                            }
                        });
                }
            }
            return result;
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
        private MatchedApplicationInfo SetAppInfo(string appName, string file)
        {
            if (Manager.MatchedAppInfos.All(appInfo => appInfo.ApplicationName != appName))
            {
                var newAppInfo = new MatchedApplicationInfo()
                {
                    ApplicationName = appName,
                    File = file
                };
                return newAppInfo;
            }

            return null;
        }
        private MatchedApplicationInfo GetExistsAppInfo([NotNull]string appName)
        {
            return Manager.MatchedAppInfos.SingleOrDefault(x => x.ApplicationName == appName);
        }

        private static bool FileNameFormatCheck(string appName)
        {
            return !Regex.IsMatch(appName, @"^(?!.*\.exe$).*$");
        }
    }
}
