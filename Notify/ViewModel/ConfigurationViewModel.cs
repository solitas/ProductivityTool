using DynamicData;
using DynamicData.Annotations;

using Microsoft.WindowsAPICodePack.Dialogs;

using ProductivityTool.Notify.Model;

using ReactiveUI;

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProductivityTool.Notify.ViewModel
{
    public class ConfigurationViewModel : ReactiveObject, IConfigurationViewModel
    {
        private const int MaxCountOfRootPath = 3;

        private readonly IComponentUpdater _componentUpdater;

        private ApplicationModel _selectedAppModel;

        public ReadOnlyObservableCollection<MatchedApplication> MatchedItems { get; }

        public ConfigurationViewModel(IComponentUpdater updater, ApplicationManager manager)
        {
            _componentUpdater = updater;
            Manager = manager;


            UpdateApp = ReactiveCommand.Create(async () =>
            {
                await UpdateApplications();
            });

            ResetAppInfo = ReactiveCommand.Create(ResetApplication);

            manager.MatchedAppInfos.Connect()
                .Filter(app => !(app is ConfigurationMenu) && !(app is ExitMenu))
                .Bind(out var items)
                .Subscribe();

            MatchedItems = items;


            AddApplication = ReactiveCommand.Create(async () =>
            {
                string applicationName = await Interactions.ApplicationFileSelect.Handle(Unit.Default);
                CreateNewApplicationModel(applicationName);
            });

            RemoveApplication = ReactiveCommand.Create(() =>
            {
                
            });

            AddRootPath = ReactiveCommand.Create(async () =>
            {
                string rootPath = await Interactions.RootPathSelect.Handle(Unit.Default);

                if (Manager.RootPaths.All(name => name != rootPath))
                {
                    Manager.RootPaths.Add(rootPath);
                }
            });

            RemoveRootPath = ReactiveCommand.Create(() =>
            {
//                if (Manager.RootPaths.Any(name => name == UserSelectRootPath))
//                {
//                    Manager.RootPaths.Remove(UserSelectRootPath);
//                }
            });
        }

        public ApplicationManager Manager { get; set; }

        public ApplicationModel SelectedAppModel
        {
            get => _selectedAppModel;
            set => this.RaiseAndSetIfChanged(ref _selectedAppModel, value);
        }
        public ICommand UpdateApp { get; set; }
        public ICommand ResetAppInfo { get; set; }
        public ICommand AddApplication { get; set; }
        public ICommand AddRootPath { get; set; }
        public ICommand RemoveRootPath { get; set; }
        public ICommand RemoveApplication { get; set; }

        private async Task UpdateApplications()
        {
            await Manager.UpdateApplication(_componentUpdater);
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
    }
}
