﻿using ProductivityTool.Notify.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive;
using System.Threading;

namespace ProductivityTool.Notify.ViewModel
{
    public class ProgramInsertViewModel : ReactiveObject, IProgramInsertViewModel
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
                Console.WriteLine("========== insert program ==========");
                Console.WriteLine($"Program Label = {ProgramLabel}");
                Console.WriteLine($"Program File = {ExecutionProgramFile}");
                Console.WriteLine($"Program RootFolder = {RootDirectory}");
                Console.WriteLine($"Copy to local = {CopyToLocal}");
                Console.WriteLine("====================================");
                // 1. Check exists file
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.WriteLine(@"add program start");
                
                var file = await FileService.SearchAsync(RootDirectory, ExecutionProgramFile, cts.Token);

                Console.WriteLine(@"file search complete");
                
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
                        FileInfo f = new FileInfo(file);
                        
                        program = new ExternalLocalProgram(ProgramLabel)
                        {
                            ExecuteDirectory = f.DirectoryName
                        };
                    }

                    program.PathToSearch = RootDirectory;
                    program.File = ExecutionProgramFile;
                }
                return program;
            });
        }
    }
}