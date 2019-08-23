using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using ProductivityTool.Notify.Model;
using ReactiveUI;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IProgramInsertViewModel
    {
        bool CopyToLocal { get; set; }
        string ProgramLabel { get; set; }
        string ExecutionProgramFile { get; set; }
        string RootDirectory { get; set; }
        ReactiveCommand<Unit, IExternalProgram> InsertProgram { get; }
    }
}
