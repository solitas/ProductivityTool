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

namespace ProductivityTool.Notify.View
{
    /// <summary>
    /// Interaction logic for QuestionWindow.xaml
    /// </summary>
    public partial class QuestionWindow
    {
        private string _appName;
        public QuestionWindow()
        {
            InitializeComponent();
        }

        public QuestionWindow(string appName)
        {
            _appName = appName;

            InitializeComponent();
            AppNameBlock.Text = _appName;
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
