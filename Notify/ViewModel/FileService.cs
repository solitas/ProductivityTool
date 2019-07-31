using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using ProductivityTool.Notify.Model;
using ProductivityTool.Notify.Properties;

namespace ProductivityTool.Notify.ViewModel
{
    public static class FileService
    {
        // last write time 기준으로 Search
        
        public static async Task<string> SearchAsync(ICollection<string> roots, string searchPattern, IComponentUpdater updater = null)
        {
            try
            {
                return await Task.Factory.StartNew(() =>
                {
                    var listFileFound = new List<string>();

                    foreach (var root in roots)
                    {
                        var rootIsExist = Directory.Exists(root);

                        if (rootIsExist)
                        {
                            FileSearch(listFileFound, root, searchPattern, updater);
                        }
                    }


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
                });
            }
            catch
            {

            }

            return string.Empty;
        }

        public static void InitializeConfigFile(ApplicationManager manager)
        {
            if (!File.Exists(Resources.ConfigurationFile))
            {
                XDocument doc = new XDocument();
                var rootElement = new XElement("Configuration");
                doc.Add(rootElement);
                doc.Save(Resources.ConfigurationFile);
            }

            LoadConfigurationFile(Resources.ConfigurationFile, manager.RootPaths, manager.ApplicationNames);
        }

        private static void FileSearch(List<string> fileFound, string dir, string searchPattern, IComponentUpdater updater = null)
        {
            try
            {
                var directories = Directory.GetDirectories(dir);

                foreach (var directory in directories)
                {
                    updater?.Update(directory);
                    try
                    {
                        var files = Directory.GetFiles(directory, searchPattern);
                        foreach (var file in files)
                        {
                            if (Regex.IsMatch(file, searchPattern))
                            {
                                fileFound.Add(file);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine(@"2.권한 없는 폴더임 {0}", directory);
                    }
                    FileSearch(fileFound, directory, searchPattern);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(@"1.권한 없는 폴더임 {0}", dir);
            }
        }

        public static bool SaveConfigurationFile(string filePath, ICollection<string> rootPaths, ICollection<string> appNames)
        {
            var rootElement = new XElement("Configuration");
            rootElement.Add(MakeCollectionElement("RootPaths", "RootPath", rootPaths));
            rootElement.Add(MakeCollectionElement("AppNames", "AppName", appNames));
            try
            {
                var doc = new XDocument();
                doc.Add(rootElement);
                doc.Save(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool LoadConfigurationFile(string filePath, ICollection<string> rootPaths, ICollection<string> appNames)
        {
            try
            {
                var doc = XDocument.Load(filePath);
                if (doc.Root == null) return false;

                var rootPathsElement = doc.Root.Element("RootPaths");
                var appNamesElement = doc.Root.Element("AppNames");

                if (rootPathsElement != null)
                {
                    foreach (var e in rootPathsElement.Elements("RootPath"))
                    {
                        rootPaths.Add(e.Value);
                    }
                }


                if (appNamesElement != null)
                {
                    foreach (var e in appNamesElement.Elements("AppName"))
                    {
                        appNames.Add(e.Value);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static XElement MakeCollectionElement(string parentName, string childName, ICollection<string> collection)
        {
            var parent = new XElement(parentName);
            foreach (var item in collection)
            {
                XElement child = new XElement(childName, item);
                parent.Add(child);
            }
            return parent;
        }
    }
}
