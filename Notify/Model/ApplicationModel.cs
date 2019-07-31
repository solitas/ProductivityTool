using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace ProductivityTool.Notify.Model
{
    public class ApplicationManager : ReactiveObject
    {
        private static readonly Lazy<ApplicationManager> Lazy = new Lazy<ApplicationManager>(()=> new ApplicationManager());

        public ObservableCollection<string> RootPaths { get; }
        public ObservableCollection<string> ApplicationNames { get; }
        public ObservableCollection<MatchedApplicationInfo> MatchedAppInfos { get; }

        public static ApplicationManager Instance => Lazy.Value;

        private ApplicationManager()
        {
            RootPaths = new ObservableCollection<string>();
            ApplicationNames = new ObservableCollection<string>();
            MatchedAppInfos = new ObservableCollection<MatchedApplicationInfo>();
        }
    }
}
