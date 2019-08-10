using System.Reactive;
using ReactiveUI;

namespace ProductivityTool.Notify
{
    public static class Interactions
    {
        public static Interaction<string, bool> QuestionUpdateApplication = new Interaction<string, bool>(RxApp.MainThreadScheduler);

        public static Interaction<Unit, string> RootPathSelect = new Interaction<Unit, string>(RxApp.MainThreadScheduler);
        public static Interaction<Unit, string> ApplicationFileSelect = new Interaction<Unit, string>(RxApp.MainThreadScheduler);

    }
}
