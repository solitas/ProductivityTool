using System;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProductivityTool.Notify.Model
{
    public class ExternalNetworkProgram : ReactiveObject, IExternalProgram
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
        public ExternalNetworkProgram()
        {

        }
        public ExternalNetworkProgram(string label)
        {
            Label = label;
        }

        [Reactive]
        public bool IsCopyAllFiles { get; set; }

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