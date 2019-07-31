using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IComponentUpdater
    {
        void Update(string message);
        void UpdateInfo(MatchedApplicationInfo newAppInfo);
    }
}
