using System;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify.ViewModel
{
    public class DirectoryNameUpdater : IComponentUpdater
    {
        public Action<string> DirectoryFieldUpdated { get; set; }
        public Action<MatchedApplication> MatchedAppUpdate { get; set; }

        public int TotalDirectoryCount { get; set; }
        public int CurrentDirectoryIndex { get; set; }

        public void Update(string message)
        {
            DirectoryFieldUpdated?.Invoke(message);
        }

        public void UpdateInfo(MatchedApplication newApp)
        {
            MatchedAppUpdate?.Invoke(newApp);
        }
    }
}
