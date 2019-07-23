using System.IO;
using ProductivityTool.Notify.Model;

namespace ProductivityTool.Notify
{
    public class AppContextService
    {
        private const string ServiceFile = "context.config";

        private ExeAppContextCollection _contexts;

        public AppContextService(ExeAppContextCollection contexts)
        {
            _contexts = contexts;

            CheckExecuteApplication();
        }

        public ExeAppContextCollection Contexts
        {
            get => _contexts;
            set => _contexts = value;
        }

        public void EnrollApplication(ExeAppContext context)
        {
            _contexts.Add(context);
            var destDirectory = $"App{_contexts.Count:D2}";

            try
            {
                if (Directory.Exists(destDirectory))
                {
                    Directory.Delete(destDirectory, true);
                }

                // copy file all
                var srcDirInfo = new DirectoryInfo(context.DirectoryPath);
                var destDirInfo = Directory.CreateDirectory($"App{_contexts.Count:D2}");
                DirectoryCopy(srcDirInfo.FullName, destDirInfo.FullName, true);
            }
            catch
            {
                context.IsValid = false;
            }
        }

        private void CheckExecuteApplication()
        {
            if (_contexts == null) return;

            foreach (var context in _contexts)
            {
                // directory and file 
                if (!File.Exists(context.FullPath))
                {
                    context.IsValid = false;
                    continue;
                }
                context.IsValid = true;
                context.Init();
            }
            _contexts.Remove(item => !item.IsValid);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
