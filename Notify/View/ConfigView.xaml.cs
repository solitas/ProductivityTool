using ProductivityTool.Notify.Model;
using ProductivityTool.Notify.ViewModel;
using ReactiveUI;

namespace ProductivityTool.Notify.View
{
    /// <summary>
    /// ConfigView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConfigView
    {
        public ConfigView()
        {
            InitializeComponent();
            IComponentUpdater updater = new DirectoryNameUpdater();
            ViewModel = new ConfigurationViewModel(updater , ApplicationManager.Instance);

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, viewModel => viewModel.Manager.RootPaths, view => view.RootPathItems.ItemsSource);
                this.OneWayBind(ViewModel, viewModel => viewModel.Manager.ApplicationNames, view => view.AppNameItems.ItemsSource);

                this.Bind(ViewModel, viewModel => viewModel.UserInputRootPath, view => view.UserInputRootPath.Text);
                this.Bind(ViewModel, viewModel => viewModel.UserInputAppName, view => view.UserInputAppName.Text);

                this.BindCommand(ViewModel, viewModel => viewModel.AddRootPath, view => view.AddRootPath);
                this.BindCommand(ViewModel, viewModel => viewModel.AddApplication, view => view.AddApplication);
                this.BindCommand(ViewModel, viewModel => viewModel.SelectPath, view => view.SelectPathButton);
            });
        }
    }
}
