using SourceChord.FluentWPF;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace reMCenters
{
    /// <summary>
    /// Interaction logic for CommonPage.xaml
    /// </summary>
    public partial class CommonPage : Page
    {
        Storyboard NotificationStoryBoard;
        public CommonPage()
        {
            InitializeComponent();
            NotificationStoryBoard = FindResource("NotificationAppear") as Storyboard;
            
        }
        public bool IsShowingNotifications { get; private set; }
        public List<Notification> NotificationQueue = new List<Notification>();
        
        public Notification AddNotificationToQueue(string title,string message)
        {
            var notification = new Notification { Title = title, Message = message };
            NotificationQueue.Add(notification);
            if (!IsShowingNotifications) { ShowNextNotification();
                IsShowingNotifications = true;
            }
            return notification;
        }
        public void ShowNextNotification()
        {
            
            var notification= NotificationQueue[0];
            NotificationQueue.RemoveAt(0);
            NotificationTitle.Text = notification.Title;
            NotificationMessage.Text = notification.Message;
            NotificationStoryBoard.Begin(notificationBox);

        }
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (NotificationQueue.Count == 0)
            {
                IsShowingNotifications = false;
                return;
            }
            ShowNextNotification();
        }

        Action DialogOKAction;
        Action DialogCancelAction;

        private void DialogCancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (DialogCancelAction != null) DialogCancelAction.Invoke();
            DialogCancelAction = null;
            DialogBox.Visibility = Visibility.Hidden;
        }

        private void DialogOkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DialogOKAction != null) DialogOKAction.Invoke();
            DialogOKAction = null;
            DialogBox.Visibility = Visibility.Hidden;
        }
        public void ShowDialog(string title,string description,string cancelButtonText,string okButtonText,Action dialogOKAction,Action dialogCancelAction)
        {
            DialogTitle.Text = title;
            DialogDescription.Text = description;
            DialogCancelButton.Content = cancelButtonText;
            DialogOkButton.Content=okButtonText;
            DialogCancelAction = dialogCancelAction;
            DialogOKAction= dialogOKAction;
            DialogBox.Visibility = Visibility.Visible;
        }
    }
}
