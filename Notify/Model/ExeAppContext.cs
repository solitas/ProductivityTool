using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Xml.Serialization;
using ReactiveUI;

namespace ProductivityTool.Notify.Model
{
    public class ExeAppContext
    {
        public string AppName { get; set; }
        public string AppExeFileName { get; set; }
        public string FullPath { get; set; }
        public string Description { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string DirectoryPath { get; set; }
        [XmlIgnore]
        public Image Icon { get; set; }
        [XmlIgnore]
        public FileVersionInfo FileVersionInfo { get; set; }
        [XmlIgnore]
        public Action Execute { get; set; }

        public bool IsValid
        {
            get; set;
        }

        public ExeAppContext()
        {
            AppName = string.Empty;
            AppExeFileName = string.Empty;
            FullPath = string.Empty;
            Description = string.Empty;
            DirectoryPath = string.Empty;
            IsValid = false;
        }

        public ExeAppContext(string path)
        {
            Initialize(path);
        }
        public void Init()
        {
            if (IsValid)
            {
                FileVersionInfo = FileVersionInfo.GetVersionInfo(FullPath);
                Execute = new Action(() =>
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(FullPath));
                    }
                    catch
                    {

                    }
                });
            }

        }

        private void Initialize(string path)
        {
            FullPath = path;

            var info = new FileInfo(path);
            var versionInfo = FileVersionInfo.GetVersionInfo(path);

            AppExeFileName = info.Name;
            FullPath = path;
            Description = versionInfo.FileDescription;
            FileVersionInfo = versionInfo;
            LastWriteTime = info.LastWriteTime;
            DirectoryPath = info.DirectoryName;

            Execute = new Action(() =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo(FullPath));
                }
                catch
                {

                }

            });
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"{AppExeFileName}");
            builder.AppendLine($"Path : {FullPath}");
            builder.AppendLine($"File Desc : {Description}");
            builder.AppendLine($"Version : {FileVersionInfo.FileVersion}");

            return builder.ToString();
        }
    }

    public class MatchedApplicationInfo : ReactiveObject
    {
        private string _applicationName;

        public string ApplicationName
        {
            get => _applicationName;
            set => this.RaiseAndSetIfChanged(ref _applicationName, value);
        }

        private string _file;

        public string File
        {
            get => _file;
            set => this.RaiseAndSetIfChanged(ref _file, value);
        }
    }
}
