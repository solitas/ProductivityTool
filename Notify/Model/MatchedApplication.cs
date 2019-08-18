using ReactiveUI;

using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProductivityTool.Notify.Model
{
    public class MatchedApplication : ReactiveObject, IMenuItem
    {
        private Guid _applicationId;

        private string _applicationName;

        private string _executeFile;

        private string _header;

        private Image _icon;

        private string _originalFile;
        
        private int _badgeValue;
        
        public MatchedApplication()
        {
            Command = ReactiveCommand.Create(() =>
            {
                if (Update != null && Update.NeedUpdate)
                {
                    var message = $"{Header} 에 대한 최신 파일이 존재합니다.\r\n최신 파일로 변경하시겠습니까?";
                    Interactions.QuestionUpdateApplication.Handle(message).Subscribe(questionResult =>
                    {
                        if (questionResult)
                        {
                            ApplicationManager.Instance.UpdateMatchedApplication(this, Update.OriginalFile);
                            BadgeValue = 0;
                            Update = null;
                            Process.Start(ExecuteFile);
                        }
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(ExecuteFile))
                    {
                        try
                        {
                            Process.Start(ExecuteFile);
                        }
                        catch
                        {

                        }
                    }
                }
            });
        }

        public MatchedApplication(string header) : this()
        {
            Header = header;
        }

        public Guid ApplicationId
        {
            get => _applicationId;
            set => this.RaiseAndSetIfChanged(ref _applicationId, value);
        }

        public string ApplicationName
        {
            get => _applicationName;
            set => this.RaiseAndSetIfChanged(ref _applicationName, value);
        }

        public string OriginalFile
        {
            get => _originalFile;
            set => this.RaiseAndSetIfChanged(ref _originalFile, value);
        }

        public string ExecuteFile
        {
            get => _executeFile;
            set => this.RaiseAndSetIfChanged(ref _executeFile, value);
        }

        public Action Execute { get; set; }

        #region implements of IMenu

        public string Header
        {
            get => _header;
            set => this.RaiseAndSetIfChanged(ref _header, value);
        }

        public ICommand Command { get; set; }

        public Image Icon
        {
            get => _icon;
            set => this.RaiseAndSetIfChanged(ref _icon, value);
        }

        #endregion

        public bool CheckValidation()
        {
            var result = false;

            if (!string.IsNullOrEmpty(_originalFile)) result = File.Exists(_originalFile);

            if (!string.IsNullOrEmpty(_executeFile)) result = File.Exists(_executeFile);

            return result;
        }

        public void SetIcon(string executeFile)
        {
            Icon = new Image
            {
                Source = FileToImageIconConverter.Icon(executeFile)
            };
        }

        public UpdateModel Update { get; set; }

        public int BadgeValue
        {
            get => _badgeValue;
            set => this.RaiseAndSetIfChanged(ref _badgeValue, value);
        }
    }
    public class UpdateModel
    {
        public bool NeedUpdate { get; set; }
        public string OriginalFile { get; set; }
    }
}