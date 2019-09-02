using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using ProductivityTool.Notify.ViewModel;

namespace ProductivityTool.Notify.Model
{
    public interface IExternalProgram : INotifyMenu
    {
        /// <summary>
        /// label for executable program
        /// </summary>
        new string Label { get; set; }

        /// <summary>
        /// program name, including directory path
        /// ex: c:/application.exe
        /// </summary>
        string File { get; set; }

        /// <summary>
        /// program name, including directory path
        /// ex: c:/application.exe
        /// </summary>
        string ExecuteDirectory { get; set; }

        /// <summary>
        /// the path to search for the program
        /// </summary>
        string PathToSearch { get; set; }

        /// <summary>
        /// the number of program runs.
        /// </summary>
        int NumOfRuns { get; set; }
        
        /// <summary>
        /// file validity
        /// </summary>
        bool IsValid { get; set; }

        bool IsSelected { get; set; }

        /// <summary>
        /// Icon Image
        /// </summary>
        new Image IconImage { get; set; }

        /// <summary>
        /// run program command
        /// </summary>
        new ICommand Execute { get; set; }

        void NeedUpdate();
        void CheckUpdate();
        void CheckValidity();
    }

    public class DummyPrograms
    {
        public List<IExternalProgram> Programs { get; set; }

        public DummyPrograms()
        {
            Programs = new List<IExternalProgram>();

            var executeProgramForLocal = new ExternalLocalProgram("myApp")
            {
                ExecuteDirectory = "C:\\Users\\Solit\\project\\myApp\\bin\\netcoreapp3.0\\",
                PathToSearch = "C:\\Users\\Solit\\project\\myApp\\",
                File = "myApp.exe",
            };
            Programs.Add(executeProgramForLocal);
        }
    }
}