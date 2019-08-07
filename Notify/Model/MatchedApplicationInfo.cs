using System;
using System.IO;
using System.Net;
using ReactiveUI;

namespace ProductivityTool.Notify.Model
{
    public class MatchedApplicationInfo : ReactiveObject
    {
        private Guid _applicationId;

        public Guid ApplicationId
        {
            get => _applicationId;
            set => this.RaiseAndSetIfChanged(ref _applicationId, value);
        }

        private string _applicationName;

        public string ApplicationName
        {
            get => _applicationName;
            set => this.RaiseAndSetIfChanged(ref _applicationName, value);
        }

        private string _originalFile;

        public string OriginalFile
        {
            get => _originalFile;
            set => this.RaiseAndSetIfChanged(ref _originalFile, value);
        }

        private string _executeFile;

        public string ExecuteFile
        {
            get => _executeFile;
            set => this.RaiseAndSetIfChanged(ref _executeFile, value);
        }

        public bool CheckValidation()
        {
            bool result = false;

            if (!string.IsNullOrEmpty(_originalFile))
            {
                result = File.Exists(_originalFile);
            }

            if (!string.IsNullOrEmpty(_executeFile))
            {
                result = File.Exists(_executeFile);
            }

            return result;
        }

        public Action Execute { get; set; }
    }
}