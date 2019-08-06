using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using ProductivityTool.Notify.ViewModel;

namespace ProductivityTool.Notify.Model
{
    public class ApplicationManager : ReactiveObject
    {
        private const string RootApplicationDirectory = "Applications\\";
        private static readonly Lazy<ApplicationManager> Lazy = new Lazy<ApplicationManager>(() => new ApplicationManager());

        public ObservableCollection<string> RootPaths { get; }
        public ObservableCollection<string> ApplicationNames { get; }
        public ObservableCollection<MatchedApplicationInfo> MatchedAppInfos { get; }

        public static ApplicationManager Instance => Lazy.Value;

        private ApplicationManager()
        {
            RootPaths = new ObservableCollection<string>();
            ApplicationNames = new ObservableCollection<string>();
            MatchedAppInfos = new ObservableCollection<MatchedApplicationInfo>();
        }

        public void CopyApplication(MatchedApplicationInfo info)
        {
            try
            {
                var fileInfo = new FileInfo(info.OriginalFile);
                var dirName = Path.GetFileNameWithoutExtension(info.OriginalFile);
                var applicationDir = $"{RootApplicationDirectory}{dirName}\\";

                FileService.DirectoryCopy(fileInfo.DirectoryName, applicationDir, true);
                info.ExecuteFile = $"{applicationDir}{info.ApplicationName}";
            }
            catch
            {
                // ignored
            }
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
    }
}
