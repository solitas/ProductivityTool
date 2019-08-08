using ReactiveUI;

namespace ProductivityTool.Notify
{
    public static class Interactions
    {
        public static Interaction<string, bool> QuestionUpdateApplication = new Interaction<string, bool>(RxApp.MainThreadScheduler);
    }
}
