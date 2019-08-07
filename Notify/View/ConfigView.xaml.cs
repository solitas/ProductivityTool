﻿using System;
using System.Windows.Threading;
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
            var updater = new DirectoryNameUpdater();
            Action<string>  directoryUpdateAction = (message) =>
            {
                DirectoryLog.Text = message;
            };
            updater.DirectoryFieldUpdated = message =>
            {
                Dispatcher.BeginInvoke(directoryUpdateAction, message);
            };
            ViewModel = new ConfigurationViewModel(updater, ApplicationManager.Instance);

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, viewModel => viewModel.Manager.RootPaths, view => view.RootPathItems.ItemsSource);
                this.OneWayBind(ViewModel, viewModel => viewModel.Manager.ApplicationModels, view => view.AppNameItems.ItemsSource);
                this.OneWayBind(ViewModel, viewModel => viewModel.Manager.MatchedAppInfos, view => view.MatchedAppItems.ItemsSource);
                this.Bind(ViewModel, viewModel => viewModel.UserInputRootPath, view => view.UserInputRootPath.Text);
                this.Bind(ViewModel, viewModel => viewModel.UserInputAppName, view => view.UserInputAppName.Text);
                this.Bind(ViewModel, viewModel => viewModel.SelectedAppModel, view => view.AppNameItems.SelectedItem);
                this.BindCommand(ViewModel, viewModel => viewModel.AddRootPath, view => view.AddRootPath);
                this.BindCommand(ViewModel, viewModel => viewModel.AddApplication, view => view.AddApplication);
                this.BindCommand(ViewModel, viewModel => viewModel.SelectPath, view => view.SelectPathButton);
                this.BindCommand(ViewModel, viewModel => viewModel.UpdateApp, view => view.UpdateButton);
                this.BindCommand(ViewModel, viewModel => viewModel.ResetAppInfo, view => view.ResetButton);
            });
        }
    }
}
