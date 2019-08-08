using ProductivityTool.Notify.ViewModel;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using System.Threading.Tasks;

namespace ProductivityTool.Notify.Model
{
    public class ApplicationManager : ReactiveObject
    {
        private const string RootApplicationDirectory = "Applications\\";
        private static readonly Lazy<ApplicationManager> Lazy = new Lazy<ApplicationManager>(() => new ApplicationManager());

        public ObservableCollection<string> RootPaths { get; }
        
        public ObservableCollection<ApplicationModel> ApplicationModels { get; }
        public ObservableCollection<MatchedApplicationInfo> MatchedAppInfos { get; }

        public static ApplicationManager Instance => Lazy.Value;
        private ApplicationManager()
        {
            RootPaths = new ObservableCollection<string>();
            
            MatchedAppInfos = new ObservableCollection<MatchedApplicationInfo>();
            ApplicationModels = new ObservableCollection<ApplicationModel>();
        }
        
        private void CreateDefaultApplicationDirectory()
        {
            try
            {
                if (!Directory.Exists(RootApplicationDirectory))
                {
                    Directory.CreateDirectory(RootApplicationDirectory);
                }
            }
            catch
            {

            }
        }

        public bool ExistsApplication(string fileName)
        {
            return ApplicationModels.Any(x => x.FileName == fileName);
        }

        public void AddNewApplication(string fileName)
        {
            if (!ExistsApplication(fileName))
            {
                ApplicationModels.Add(new ApplicationModel(fileName));
            }
        }

        public void RemoveApplication(Guid id)
        {
            var remove = ApplicationModels.FirstOrDefault(model => model.Id == id);
            ApplicationModels.Remove(remove);
        }

        public void InsertMatchedApplication(MatchedApplicationInfo info)
        {
            if (MatchedAppInfos.All(i => i.ApplicationId != info.ApplicationId))
            {
                var fileInfo = new FileInfo(info.OriginalFile);
                var dirName = Path.GetFileNameWithoutExtension(info.OriginalFile);
                var applicationDir = $"{RootApplicationDirectory}{dirName}\\";

                FileService.DirectoryCopy(fileInfo.DirectoryName, applicationDir, true);
                info.ExecuteFile = $"{applicationDir}{info.ApplicationName}";

                Observable.Start(() =>
                {
                    MatchedAppInfos.Add(info);
                }, RxApp.MainThreadScheduler);
            }
            else
            {
                var targetInfo = MatchedAppInfos.FirstOrDefault(i => i.ApplicationId == info.ApplicationId);
                if (targetInfo != null)
                {
                    targetInfo.OriginalFile = info.OriginalFile;
                    var fileInfo = new FileInfo(targetInfo.OriginalFile);
                    var dirName = Path.GetFileNameWithoutExtension(info.OriginalFile);
                    var applicationDir = $"{RootApplicationDirectory}{dirName}\\";

                    FileService.DirectoryCopy(fileInfo.DirectoryName, applicationDir, true);
                    info.ExecuteFile = $"{applicationDir}{info.ApplicationName}";
                }
            }
        }

        public MatchedApplicationInfo GetMatchedApplicationInfo(Guid id)
        {
            return MatchedAppInfos.FirstOrDefault(i => i.ApplicationId == id);
        }

        public void MatchedAppClear()
        {
            MatchedAppInfos.Clear();
        }

        public async Task UpdateApplication(IComponentUpdater updater)
        {
            foreach (var model in ApplicationModels)
            {
                await FileService.SearchAsync(RootPaths, model.FileName, updater)
                    .ContinueWith(t =>
                    {
                        var file = t.Result;
                        if (!string.IsNullOrEmpty(file))
                        {
                            var newAppInfo = CreateMachedApplication(model.Id, model.FileName, file);
                            InsertMatchedApplication(newAppInfo);
                        }
                    });
            }
        }
        private MatchedApplicationInfo CreateMachedApplication(Guid appId, string appName, string file)
        {
            if (MatchedAppInfos.All(appInfo => appInfo.ApplicationName != appName))
            {
                var newAppInfo = new MatchedApplicationInfo()
                {
                    ApplicationId = appId,
                    ApplicationName = appName,
                    OriginalFile = file
                };
                return newAppInfo;
            }

            return null;
        }
    }

    public class ApplicationModel
    {
        public Guid Id { get; }
        public string FileName { get; }

        public ApplicationModel()
        {
            Id = Guid.NewGuid();
        }
        public ApplicationModel(string fileName) : this()
        {
            FileName = fileName;
        }

        public ApplicationModel(Guid id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }
    }
}
