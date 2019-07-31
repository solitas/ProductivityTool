using System;

namespace ProductivityTool.Notify.ViewModel
{
    public class DirectoryNameUpdater : IComponentUpdater
    {
        public Action<string> InvokeUpdate { get; set; }
        public void Update(string message)
        {
            InvokeUpdate?.Invoke(message);
        }
    }
}
