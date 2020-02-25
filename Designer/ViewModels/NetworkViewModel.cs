using System.Reactive.Linq;
using ReactiveUI;

namespace Designer.ViewModels
{
    public class NetworkViewModel : ReactiveObject
    {
        private double _zoomRate = 1.0;

        public NetworkViewModel()
        {

            SetZoom = ReactiveCommand.CreateFromObservable<double, bool>(zoomRate =>
            {

                return Observable.Return(true);
            });
        }

        public ReactiveCommand<double, bool> SetZoom { get; }
    }
}