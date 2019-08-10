using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IComponentUpdater
    {
        int TotalDirectoryCount { get; set; }
        int CurrentDirectoryIndex { get; set; }
        void Update(string message);
    }
}
