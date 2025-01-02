using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace reMCenters
{
    /// <summary>
    /// Interaction logic for ErrorScreen.xaml
    /// </summary>
    /// 


    public partial class ErrorScreen : Page
    {

        public ErrorScreen()
        {
            InitializeComponent();
            cancelButton.ConnectedImage = cancelIcon;
            copyButton.ConnectedImage = copyIcon;
            retryButton.ConnectedImage = retryIcon;

        }

        public void RemoveCancelHandles()
        {
            if (CancelClicked == null)
                return;
            try
            {
                foreach (var g in CancelClicked.GetInvocationList())
                {
                    CancelClicked -= (EventHandler)g;

                }
            }
            catch (System.NullReferenceException) { }
        }

        public void RemoveCopyClipboardHandles()
        {
            if (CopyClicked == null)
                return;
            try
            {
                foreach (var g in CopyClicked.GetInvocationList())
                {
                    CopyClicked -= (EventHandler)g;

                }
            }
            catch (System.NullReferenceException) { }
        }
        public void RemoveAllHandles()
        {
            RemoveRetryHandles();
            RemoveCopyClipboardHandles();
            RemoveCancelHandles();
        }

        public void Reset()
        {
            RemoveAllHandles();
            ErrorDescription = "Error Details";
            ErrorTitle = "Error Title";
            ErrorSubTitle = "Error Line";
            copyButton.Content = "Copy to Clipboard";
            retryButton.Content = "Retry";
            cancelButton.Content = "Cancel";
            RetryVisible = true;
            CopyVisible = true;

        }
        public void RemoveRetryHandles()
        {
            if (RetryClicked == null)
                return;
            try
            {
                foreach (var g in RetryClicked.GetInvocationList())
                {
                    RetryClicked -= (EventHandler)g;

                }
            }
            catch (System.NullReferenceException) { }
        }

        public bool RetryVisible
        {
            set
            {
                retryButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                retryIcon.Visibility = retryButton.Visibility;
            }
        }
        public bool CopyVisible
        {
            set
            {
                copyButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                copyIcon.Visibility = retryButton.Visibility;
            }
        }
        
        public string ErrorTitle { get { return errorTitle.Text; } set { errorTitle.Text = value; } }
        public string ErrorSubTitle { get { return errorLine.Text; } set { errorLine.Text = value; } }
        public string ErrorDescription { get { return errorDescription.Text; } set { errorDescription.Text = value; } }
        public event EventHandler CopyClicked;
        public event EventHandler RetryClicked;
        public event EventHandler CancelClicked;
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelClicked?.Invoke(null, null);
        }

        readonly ErrorTypeEnum mode = ErrorTypeEnum.Common;
        public ErrorTypeEnum CurrentMode
        {
            get { return mode; }
            set
            {
                if (value == mode)
                    return;
                switch (value)
                {



                    case ErrorTypeEnum.ReportDll:
                        copyButton.Content = "Copy DLLs and join the OpenM discord";
                        break;
                    default:
                        copyButton.Content = "Copy to clipboard";
                        break;

                }

            }
        }

        public string CancelButtonText
        {
            get

            {
                return cancelButton.Content.ToString();

            }
            set { cancelButton.Content = value; }
        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            CopyClicked?.Invoke(null, null);
        }



        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            RetryClicked?.Invoke(null, null);
        }




    }
}
