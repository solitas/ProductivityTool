using DynamicData;
using DynamicData.Annotations;

using ProductivityTool.Notify.Model;

using ReactiveUI;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProductivityTool.Notify.ViewModel
{
    public class ConfigurationViewModel : ReactiveObject, IConfigurationViewModel, IDisposable
    {
        private const int MaxCountOfRootPath = 3;

        private readonly IComponentUpdater _componentUpdater;
        private string _selectedRootPath;
        private ApplicationModel _selectedAppModel;
        private MatchedApplication _selectedMatchedApplication;
        private bool _isUpdating;
        private CancellationTokenSource _ctx;

        public bool IsUpdating
        {
            get => _isUpdating;
            set => this.RaiseAndSetIfChanged(ref _isUpdating, value);
        }

        public ReadOnlyObservableCollection<MatchedApplication> MatchedItems { get; }

        public ConfigurationViewModel(IComponentUpdater updater, ApplicationManager manager)
        {
            _componentUpdater = updater;
            Manager = manager;

            manager.MatchedAppInfos.Connect()
                .Filter(app => !(app is ConfigurationMenu) && !(app is ExitMenu))
                .Bind(out var items)
                .Subscribe();

            MatchedItems = items;

            var canUpdate = this.WhenAnyValue(x => x.IsUpdating).Select(isUpdating => isUpdating == false);
            var canRemoveApplication = this.WhenAnyValue(x => x.SelectedAppModel).Select(x => x != null && !IsUpdating);
            var canRemoveRootPath = this.WhenAnyValue(x => x.SelectedRootPath).Select(x => !string.IsNullOrEmpty(x) && !IsUpdating);
            _ctx = new CancellationTokenSource();

            UpdateApp = ReactiveCommand.Create(async () =>
            {
                ApplicationManager.Instance.StopUpdaterCheck();
                Console.WriteLine(@"Update App Starting ..");
                IsUpdating = true;
                await UpdateApplications(_ctx.Token).ContinueWith(x =>
                {
                    
                });
                Console.WriteLine(@"Update App Stopped ..");
                ApplicationManager.Instance.StartUpdateChecker();
                IsUpdating = false;
            }, canUpdate);

            ResetAppInfo = ReactiveCommand.Create(ResetApplication, canUpdate);

            AddApplication = ReactiveCommand.Create(async () =>
            {
                string applicationName = await Interactions.ApplicationFileSelect.Handle(Unit.Default);
                CreateNewApplicationModel(applicationName);
            }, canUpdate);


            RemoveApplication = ReactiveCommand.Create(() =>
            {
                if (SelectedAppModel != null)
                {
                    Manager.ApplicationModels.Remove(SelectedAppModel);
                }
            }, canRemoveApplication);

            AddRootPath = ReactiveCommand.Create(async () =>
            {
                string rootPath = await Interactions.RootPathSelect.Handle(Unit.Default);

                if (Manager.RootPaths.All(name => name != rootPath))
                {
                    Manager.RootPaths.Add(rootPath);
                }
            }, canUpdate);

            RemoveRootPath = ReactiveCommand.Create(() =>
            {
                if (!string.IsNullOrEmpty(SelectedRootPath))
                {
                    Manager.RootPaths.Remove(SelectedRootPath);
                }
            }, canRemoveRootPath);

            this.WhenAnyValue(x => x.IsUpdating)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(isUpdating =>
                {
                    if (isUpdating)
                    {

                    }
                });
        }

        public ApplicationManager Manager { get; set; }
        public string SelectedRootPath
        {
            get => _selectedRootPath;
            set => this.RaiseAndSetIfChanged(ref _selectedRootPath, value);
        }
        public ApplicationModel SelectedAppModel
        {
            get => _selectedAppModel;
            set => this.RaiseAndSetIfChanged(ref _selectedAppModel, value);
        }
        public MatchedApplication SelectedMatchedApplication
        {
            get => _selectedMatchedApplication;
            set => this.RaiseAndSetIfChanged(ref _selectedMatchedApplication, value);
        }
        public ICommand UpdateApp { get; set; }
        public ICommand ResetAppInfo { get; set; }
        public ICommand AddApplication { get; set; }
        public ICommand AddRootPath { get; set; }
        public ICommand RemoveRootPath { get; set; }
        public ICommand RemoveApplication { get; set; }
        public ICommand SelectPath { get; set; }

        private async Task UpdateApplications(CancellationToken token)
        {
            await Manager.UpdateApplication(token, _componentUpdater);
        }
        private void ResetApplication()
        {
            Manager.MatchedAppClear();
        }
        private void CreateNewApplicationModel(string fileName)
        {
            Manager.AddNewApplication(fileName);
        }
        private void RemoveApplicationModel(Guid appId)
        {
            Manager.RemoveApplication(appId);
        }
        private MatchedApplication SetAppInfo(Guid appId, string appName, string file)
        {
            if (Manager.MatchedAppInfos.Items.All(appInfo => appInfo.ApplicationName != appName))
            {
                var newAppInfo = new MatchedApplication()
                {
                    ApplicationId = appId,
                    ApplicationName = appName,
                    OriginalFile = file
                };
                return newAppInfo;
            }

            return null;
        }
        private MatchedApplication GetExistsAppInfo([NotNull]string appName)
        {
            return Manager.MatchedAppInfos.Items.SingleOrDefault(x => x.ApplicationName == appName);
        }

        private static bool FileNameFormatCheck(string appName)
        {
            return !Regex.IsMatch(appName, @"^(?!.*\.exe$).*$");
        }

        public void Dispose()
        {
            _ctx?.Dispose();
        }
    }
}
