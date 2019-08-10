using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ProductivityTool.Notify.Model;
using ProductivityTool.Notify.ViewModel;
using ReactiveUI;

namespace ProductivityTool.Notify
{
    public class ApplicationManager : ReactiveObject
    {
        private const string RootApplicationDirectory = "Applications\\";
        private static readonly Lazy<ApplicationManager> Lazy = new Lazy<ApplicationManager>(() => new ApplicationManager());

        public static ApplicationManager Instance => Lazy.Value;

        private ApplicationManager()
        {
            RootPaths = new ObservableCollection<string>();
            
            MatchedAppInfos = new SourceList<MatchedApplication>();
            ApplicationModels = new ObservableCollection<ApplicationModel>();
        }

        public ObservableCollection<string> RootPaths { get; }
        public ObservableCollection<ApplicationModel> ApplicationModels { get; }
        public SourceList<MatchedApplication> MatchedAppInfos { get; }

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
                // ignored
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

        public void InsertMatchedApplication(MatchedApplication info)
        {
            if (MatchedAppInfos.Items.All(i => i.ApplicationId != info.ApplicationId))
            {
                var fileInfo = new FileInfo(info.OriginalFile);
                var dirName = Path.GetFileNameWithoutExtension(info.OriginalFile);
                var applicationDir = $"{RootApplicationDirectory}{dirName}\\";

                FileService.DirectoryCopy(fileInfo.DirectoryName, applicationDir, true);
                info.ExecuteFile = $"{applicationDir}{info.ApplicationName}";

                Observable.Start(() =>
                {
                    info.SetIcon(info.ExecuteFile);
                    MatchedAppInfos.Add(info);
                }, RxApp.MainThreadScheduler);
            }
            else
            {
                
                var targetInfo = MatchedAppInfos.Items.FirstOrDefault(i => i.ApplicationId == info.ApplicationId);
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

        public MatchedApplication GetMatchedApplicationInfo(Guid id)
        {
            return MatchedAppInfos.Items.FirstOrDefault(i => i.ApplicationId == id);
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
                            var newAppInfo = CreateMatchedApplication(model.Id, model.FileName, file);
                            if (newAppInfo != null)
                                InsertMatchedApplication(newAppInfo);
                        }
                    });
            }
        }

        private MatchedApplication CreateMatchedApplication(Guid appId, string fileName, string file)
        {
            if (MatchedAppInfos.Items.All(appInfo => appInfo.ApplicationName != fileName))
            {
                var header = Path.GetFileNameWithoutExtension(fileName);
                var newAppInfo = new MatchedApplication(header)
                {
                    ApplicationId = appId,
                    ApplicationName = fileName,
                    OriginalFile = file
                };
                return newAppInfo;
            }

            return null;
        }
        /// <summary>
        /// 최신 프로그램이 존재여부를 검사합니다.
        /// </summary>
        /// <param name="app"></param>
        /// <returns>최신 프로그램이 존재하면 최신파일경로, 그렇지 않으면 string.Empty</returns>
        public async Task<string> UpdateCheck(MatchedApplication app)
        {
            return await FileService.SearchAsync(RootPaths, app.ApplicationName)
                .ContinueWith(t =>
                {
                    var searchedFile = t.Result;
                    var appFileInfo = new FileInfo(app.ExecuteFile);
                    var searchedFileInfo = new FileInfo(searchedFile);

                    if (appFileInfo.LastWriteTime < searchedFileInfo.LastWriteTime)
                    {
                        return searchedFile;
                    }

                    return string.Empty;
                });
        }

        public void UpdateMatchedApplication(MatchedApplication app, string orgFile)
        {
            try
            {
                var fileInfo = new FileInfo(orgFile);
                var applicationDir = Path.GetDirectoryName(app.ExecuteFile);

                FileService.DirectoryCopy(fileInfo.DirectoryName, applicationDir, true);
                app.OriginalFile = orgFile;
            }
            catch
            {

            }
        }
    }
}
