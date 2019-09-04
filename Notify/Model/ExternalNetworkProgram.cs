using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProductivityTool.Notify.Model
{
    public class ExternalNetworkProgram : ReactiveObject, IExternalProgram
    {
        private string _orgFile;
        private bool _update;
        [Reactive]
        public string Label { get; set; }
        [Reactive]
        public string File { get; set; }
        [Reactive]
        public string ExecuteDirectory { get; set; }
        [Reactive]
        public string PathToSearch { get; set; }
        [Reactive]
        public int NumOfRuns { get; set; }
        [Reactive]
        public bool IsValid { get; set; }
        [Reactive]
        public Image IconImage { get; set; }
        [Reactive]
        public bool IsSelected { get; set; }

        public ICommand Execute { get; set; }
        private int _badgeValue;
        public int BadgeValue
        {
            get => _badgeValue;
            set => this.RaiseAndSetIfChanged(ref _badgeValue, value);
        }
        public ExternalNetworkProgram()
        {
            Execute = ReactiveCommand.Create(() =>
            {
                if (_update)
                {
                    Interactions.QuestionUpdateApplication.Handle(Label).Subscribe(questionResult =>
                    {
                        if (questionResult)
                        {
                            ApplicationManager.Instance.UpdateMatchedApplication(this, _orgFile);
                            BadgeValue = 0;
                            _update = false;
                            _orgFile = null;
                            Process.Start(File);
                        }
                    });
                }
                else if (!string.IsNullOrEmpty(File))
                {
                    Process.Start(File);
                }
            });
        }
        public ExternalNetworkProgram(string label)
        {
            Label = label;

            this.WhenAnyValue(x => x.ExecuteDirectory, x => x.File)
                .Subscribe(x =>
                {
                    var (dir, prg) = x;
                    if (!string.IsNullOrEmpty(prg) && !string.IsNullOrEmpty(dir))
                    {
                        // todo Action Execute
                    }
                });

            Execute = ReactiveCommand.Create(() =>
            {
                if (_update)
                {
                    var message = $"{Label} 에 대한 최신 파일이 존재합니다.\r\n최신 파일로 변경하시겠습니까?";
                    Interactions.QuestionUpdateApplication.Handle(message).Subscribe(questionResult =>
                    {
                        if (questionResult)
                        {
                            ApplicationManager.Instance.UpdateMatchedApplication(this, _orgFile);
                            BadgeValue = 0;
                            _update = false;
                            _orgFile = null;
                            Process.Start(File);
                        }
                    });
                }
                else if (!string.IsNullOrEmpty(File))
                {
                    Process.Start(File);
                }
            });
        }

        [Reactive]
        public bool IsCopyAllFiles { get; set; }

        public void NeedUpdate(bool update, string orgFile = null)
        {
            _update = update;
            _orgFile = orgFile;
        }

        public void CheckUpdate()
        {
            throw new NotImplementedException();
        }

        public void CheckValidity()
        {
            throw new NotImplementedException();
        }
    }
}