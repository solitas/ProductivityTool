using System;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify.ViewModel
{
    public class DirectoryNameUpdater : IComponentUpdater
    {
        public Action<string> DirectoryFieldUpdated { get; set; }
        public Action<MatchedApplicationInfo> MatchedAppUpdate { get; set; }
        public void Update(string message)
        {
            DirectoryFieldUpdated?.Invoke(message);
        }

        public void UpdateInfo(MatchedApplicationInfo newAppInfo)
        {
            MatchedAppUpdate?.Invoke(newAppInfo);
        }
    }
}
