using System;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProductivityTool.Notify.Model
{
    public class ExternalLocalProgram : ReactiveObject, IExternalProgram
    {
        [Reactive]
        public string Label { get; set; }
        [Reactive]
        public string Program { get; set; }
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

        public ExternalLocalProgram()
        {

        }

        public ExternalLocalProgram(string label)
        {
            Label = label;

            this.WhenAnyValue(x => x.Program, x => x.ExecuteDirectory)
                .Subscribe(x =>
                {
                    var (program, dir) = x;
                    // TODO: validation check of executable file
                });
        }

        public void NeedUpdate()
        {
            throw new System.NotImplementedException();
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