using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ProductivityTool.Notify.ViewModel;
using ReactiveUI;

namespace ProductivityTool.Notify
{
    /// <summary>
    /// Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class ConfigView 
    {
        public ConfigView()
        {
            InitializeComponent();
            
            var viewUpdater = new DirectoryNameUpdater();
            ViewModel = new ConfigurationViewModel(viewUpdater);

            this.WhenActivated(d =>
            {

            });
        }
    }
}
