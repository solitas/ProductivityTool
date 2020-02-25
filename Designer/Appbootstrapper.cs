using Designer.ViewModels;
using ReactiveUI;

using Splat;
using System.Windows;

namespace Designer
{
    public class Appbootstrapper : ReactiveObject
    {
        public Appbootstrapper(IMutableDependencyResolver dependencyResolver = null)
        {
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;
            RegisterParts(dependencyResolver);

            if (Locator.Current.GetService(typeof(IViewFor<AppViewModel>)) is ReactiveWindow<AppViewModel> window)
            {
                var viewModel = new AppViewModel();
                window.ViewModel = viewModel; 
                window.Show();
            }
        }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new MainWindow(), typeof(IViewFor<AppViewModel>));
        }
    }
}