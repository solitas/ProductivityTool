using System.Windows;

namespace ProductivityTool.Notify.View
{
    /// <summary>
    /// ApplicationFileSelectView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ApplicationFileSelectView
    {
        public string ApplicationFile => this.ApplicationFileBox.Text;

        public ApplicationFileSelectView()
        {
            InitializeComponent();
        }

        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
