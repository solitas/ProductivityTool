using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;

namespace ProductivityTool.Notify.View
{
    /// <summary>
    /// UpdateApplicationNotify.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UpdateApplicationNotify : UserControl
    {
        private bool isClosing = false;

        #region NotifyText dependency property

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty NotifyTextProperty =
            DependencyProperty.Register("NotifyText",
                typeof(string),
                typeof(UpdateApplicationNotify),
                new FrameworkPropertyMetadata("App is Updated"));

        /// <summary>
        /// A property wrapper for the <see cref="NotifyTextProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string NotifyText
        {
            get { return (string)GetValue(NotifyTextProperty); }
            set { SetValue(NotifyTextProperty, value); }
        }

        #endregion

        public Action UpdateAction { get; set; }
        public UpdateApplicationNotify()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }
        /// <summary>
        /// By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent"/>
        /// and setting the "Handled" property to true, we suppress the popup
        /// from being closed in order to display the custom fade-out animation.
        /// </summary>
        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true; //suppresses the popup from being closed immediately
            isClosing = true;
        }


        /// <summary>
        /// Resolves the <see cref="TaskbarIcon"/> that displayed
        /// the balloon and requests a close action.
        /// </summary>
        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }

        /// <summary>
        /// If the users hovers over the balloon, we don't close it.
        /// </summary>
        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            //if we're already running the fade-out animation, do not interrupt anymore
            //(makes things too complicated for the sample)
            if (isClosing) return;

            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
        }
        private void CloseButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            Popup pp = (Popup)Parent;
            pp.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateAction?.Invoke();

            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
