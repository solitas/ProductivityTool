using System;
using System.IO;
using System.Reactive;
using System.Threading;
using ProductivityTool.Notify.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProductivityTool.Notify.ViewModel
{
    public class ProgramInsertViewModel : IProgramInsertViewModel
    {
        [Reactive]
        public bool CopyToLocal { get; set; }

        [Reactive]
        public string ProgramLabel { get; set; }
        
        [Reactive]
        public string ExecutionProgramFile { get; set; }
        
        [Reactive]
        public string RootDirectory { get; set; }
        
        public ReactiveCommand<Unit, IExternalProgram> InsertProgram { get; }

        public ProgramInsertViewModel()
        {
            InsertProgram = ReactiveCommand.CreateFromTask<Unit, IExternalProgram>(async (_) =>
            {
                IExternalProgram program = null;

                // 1. Check exists file
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.WriteLine("add program start");
                await FileService.SearchAsync(RootDirectory, ExecutionProgramFile, cts.Token)
                    .ContinueWith(task =>
                    {
                        Console.WriteLine("file search complete");
                        var file = task.Result;
                        if (!string.IsNullOrEmpty(file))
                        {
                            var info = new FileInfo(file);

                            if (CopyToLocal)
                            {
                                // try to copy file 
                                var dirName = Path.GetFileNameWithoutExtension(ExecutionProgramFile);
                                var applicationDir =
                                    $"{Environment.CurrentDirectory}\\{FileService.RootApplicationDirectory}{dirName}\\";

                                FileService.DirectoryCopy(info.DirectoryName, applicationDir, true);

                                program = new ExternalNetworkProgram(ProgramLabel)
                                {
                                    ExecuteDirectory = applicationDir,
                                };
                            }
                            else
                            {
                                program = new ExternalLocalProgram(ProgramLabel)
                                {
                                    ExecuteDirectory = info.DirectoryName
                                };
                            }

                            program.PathToSearch = RootDirectory;
                            program.File = ExecutionProgramFile;
                        }
                    }, cts.Token);

                return program;
            });
        }
    }
}