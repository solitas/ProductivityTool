using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace ProductivityTool.Notify
{
    public static class Interactions
    {
        public static Interaction<string, bool> QuestionUpdateApplication = new Interaction<string, bool>(RxApp.MainThreadScheduler);

        public static Interaction<Unit, string> RootPathSelect = new Interaction<Unit, string>(RxApp.MainThreadScheduler);
        public static Interaction<Unit, string> ApplicationFileSelect = new Interaction<Unit, string>(RxApp.MainThreadScheduler);
        public static IDisposable RegisterHandlerForDialog<TInput, TOutput>(this Interaction<TInput, TOutput> interaction, Action<InteractionContext<TInput, TOutput>> handler)
        {
            return interaction.RegisterHandler(inter => Observable.Start(() =>
                {
                    handler.Invoke(inter);
                    return Unit.Default;
                },
                RxApp.MainThreadScheduler));
        }
    }
}
