using DynamicData.Annotations;
using ProductivityTool.Notify.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IConfigurationViewModel
    {
        ObservableCollection<string> RootPaths { get; }
        ObservableCollection<string> ApplicationNames { get; }
        ObservableCollection<MatchedApplicationInfo> MatchedAppInfos { get; }

        string UserInputAppName { get; set; }
        string UserInputRootPath { get; set; }
        string UserSelectAppName { get; set; }
        string UserSelectRootPath { get; set; }

        ICommand UpdateApp { get; set; }
        ICommand ResetAppInfo { get; set; }
        ICommand AddApplication { get; set; }
        ICommand RemoveApplication { get; set; }
        ICommand AddRootPath { get; set; }
        ICommand RemoveRootPath { get; set; }
    }

    public class ConfigurationViewModel : ReactiveObject, IConfigurationViewModel
    {
        private const string ConfigFile = "Configuration";
        private const int MaxCountOfRootPath = 3;
        private readonly IComponentUpdater _componentUpdater;
        
        private string _userInputAppName;
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

        public ConfigurationViewModel(IComponentUpdater updater)
        {
            _componentUpdater = updater;

            RootPaths = new ObservableCollection<string>();
            ApplicationNames = new ObservableCollection<string>();
            MatchedAppInfos = new ObservableCollection<MatchedApplicationInfo>();

            LoadConfigFile();

            UpdateApp = ReactiveCommand.Create(UpdateApplications);
            ResetAppInfo = ReactiveCommand.Create(ResetApplication);

            var canAddApp = this.WhenAnyValue(x => x.UserInputAppName)
                                .Select(x => x != null);
            var canRemoveApp = this.WhenAnyValue(x => x.UserSelectAppName)
                .Select(x => x != null);

            var canAddRootPath = this.WhenAnyValue(x => x.UserInputRootPath)
                .Select(x => x != null);
            var canRemoveRootPath = this.WhenAnyValue(x => x.UserSelectRootPath)
                .Select(x => x != null);
           
            AddApplication = ReactiveCommand.Create(() => { AddApp(UserInputAppName); }, canAddApp);
            
            RemoveApplication = ReactiveCommand.Create(() => { RemoveApp(UserSelectAppName);}, canRemoveApp);

            AddRootPath = ReactiveCommand.Create(() =>
            {
                if (RootPaths.All(name => name != UserInputRootPath))
                {
                    RootPaths.Add(UserInputRootPath);
                }
            }, canAddRootPath);
            RemoveRootPath = ReactiveCommand.Create(() =>
            {
                if (RootPaths.Any(name => name == UserSelectRootPath))
                {
                    RootPaths.Remove(UserSelectRootPath);
                }
            }, canRemoveRootPath);
        }

        public ObservableCollection<string> RootPaths { get; }
        public ObservableCollection<string> ApplicationNames { get; }
        public ObservableCollection<MatchedApplicationInfo> MatchedAppInfos { get; }

        public void SaveConfigFile()
        {
            if (FileService.SaveConfigurationFile(ConfigFile, RootPaths, ApplicationNames))
            {

            }
        }
        public void LoadConfigFile()
        {
            if (FileService.LoadConfigurationFile(ConfigFile, RootPaths, ApplicationNames))
            {
                UpdateApplications();
            }
        }

        public ICommand UpdateApp { get; set; }
        public ICommand ResetAppInfo { get; set; }
        public ICommand AddApplication { get; set; }
        public ICommand AddRootPath { get; set; }
        public ICommand RemoveRootPath { get; set; }
        public ICommand RemoveApplication { get; set; }

        private async void UpdateApplications()
        {
            foreach (var root in RootPaths)
            {
                foreach (var appName in ApplicationNames)
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
            MatchedAppInfos.Clear();
        }
        private void AddApp(string appName)
        {
            if (ApplicationNames.All(name => name != appName))
            {
                ApplicationNames.Add(appName);
            }
        }
        private void RemoveApp(string appName)
        {
            if (ApplicationNames.Any(name => name == appName))
            {
                ApplicationNames.Remove(appName);
            }
        }
        private void SetAppInfo(string appName, string file)
        {
            if (MatchedAppInfos.All(appInfo => appInfo.ApplicationName != appName))
            {
                var newAppInfo = new MatchedApplicationInfo()
                {
                    ApplicationName = appName,
                    File = file
                };
                MatchedAppInfos.Add(newAppInfo);
            }
        }
        private MatchedApplicationInfo GetExistsAppInfo([NotNull]string appName)
        {
            return MatchedAppInfos.SingleOrDefault(x => x.ApplicationName == appName);
        }
        private bool FileNameFormatCheck(string appName)
        {
            return Regex.IsMatch(appName, @"^(?!.*\.exe$).*$");
        }
    }

    public static class FileService
    {
        // last write time 기준으로 Search
        public static async Task<string> SearchAsync(string root, string searchPattern, IComponentUpdater updater = null)
        {
            try
            {
                return await Task.Factory.StartNew(() =>
                 {
                     var rootIsExist = Directory.Exists(root);

                     if (rootIsExist)
                     {
                         var listFileFound = new List<string>();

                         FileSearch(listFileFound, root, searchPattern, updater);

                         var maxLastWriteTime = DateTime.MinValue;
                         var lastWriteFile = string.Empty;

                         foreach (var file in listFileFound)
                         {
                             FileInfo fileInfo = new FileInfo(file);
                             if (fileInfo.LastWriteTime > maxLastWriteTime)
                             {
                                 maxLastWriteTime = fileInfo.LastWriteTime;
                                 lastWriteFile = file;
                             }
                         }

                         return lastWriteFile;
                     }
                     return string.Empty;
                 });
            }
            catch
            {

            }

            return string.Empty;
        }
        private static void FileSearch(List<string> fileFound, string dir, string searchPattern, IComponentUpdater updater = null)
        {
            var directories = Directory.GetDirectories(dir);

            foreach (var directory in directories)
            {
                updater?.Update(directory);

                var files = Directory.GetFiles(directory, searchPattern);
                foreach (var file in files)
                {
                    if (Regex.IsMatch(file, searchPattern))
                    {
                        fileFound.Add(file);
                    }
                    FileSearch(fileFound, directory, searchPattern);
                }
            }
        }

        public static bool SaveConfigurationFile(string filePath, ICollection<string> rootPaths, ICollection<string> appNames)
        {
            var rootElement = new XElement("Configuration");
            rootElement.Add(MakeCollectionElement("RootPaths", "RootPath", rootPaths));
            rootElement.Add(MakeCollectionElement("AppNames", "AppName", appNames));
            try
            {
                var doc = new XDocument();
                doc.Add(rootElement);
                doc.Save(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool LoadConfigurationFile(string filePath, ICollection<string> rootPaths, ICollection<string> appNames)
        {
            try
            {
                var doc = XDocument.Load(filePath);
                if (doc.Root == null) return false;

                var rootPathsElement = doc.Root.Element("RootPaths");
                var appNamesElement = doc.Root.Element("AppNames");

                if (rootPathsElement != null)
                {
                    foreach (var e in rootPathsElement.Elements("RootPath"))
                    {
                        rootPaths.Add(e.Value);
                    }
                }


                if (appNamesElement != null)
                {
                    foreach (var e in appNamesElement.Elements("AppName"))
                    {
                        appNames.Add(e.Value);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static XElement MakeCollectionElement(string parentName, string childName, ICollection<string> collection)
        {
            var parent = new XElement(parentName);
            foreach (var item in collection)
            {
                XElement child = new XElement(childName, item);
                parent.Add(child);
            }
            return parent;
        }
    }

    public interface IComponentUpdater
    {
        void Update(string message);
    }

    public class DirectoryNameUpdater : IComponentUpdater
    {
        public Action<string> InvokeUpdate { get; set; }
        public void Update(string message)
        {
            InvokeUpdate?.Invoke(message);
        }
    }
}
