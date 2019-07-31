using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProductivityTool.Notify.ViewModel
{
    public interface IConfigurationViewModel
    {
    }

    public class ConfigurationViewModel : ReactiveObject, IConfigurationViewModel
    {

    }

    public class SearchFileService
    {
        private readonly string _root;
        private readonly string _searchPattern;

        public SearchFileService(string root, string searchPattern)
        {
            _root = root;
            _searchPattern = searchPattern;
        }
        // last write time 기준으로 Search
        public async Task<string> RunAsync()
        {
            try
            {
                return await Task.Factory.StartNew(() =>
                 {
                     var rootIsExist = Directory.Exists(_root);

                     if (rootIsExist)
                     {
                         var listFileFound = new List<string>();

                         FileSearch(listFileFound, _root, _searchPattern);

                         var maxLastWriteTime = DateTime.MinValue;
                         var lastWriteFile = string.Empty;
                         
                         foreach (var file in listFileFound)
                         {
                             FileInfo fileInfo = new FileInfo(file);
                             if (fileInfo.LastWriteTime > maxLastWriteTime)
                             {
                                 maxLastWriteTime = fileInfo.LastWriteTime;
                                 lastWriteFile = file;
                             }
                         }
                         
                         return lastWriteFile;
                     }
                     return string.Empty;
                 });   
            }
            catch
            {

            }
            
            return string.Empty;
        }


        private void FileSearch(List<string> fileFound, string dir, string searchPattern)
        {
            var files = Directory.GetFiles(dir, searchPattern);
            var directories = Directory.GetDirectories(dir, searchPattern);

            foreach (var directory in directories)
            {
                foreach (var file in Directory.GetFiles(directory, searchPattern))
                {
                    
                    if (file.Contains(searchPattern))
                    {
                        fileFound.Add(file);
                        
                    }
                    FileSearch(fileFound, directory, searchPattern);
                }
            }
        }
    }
}
