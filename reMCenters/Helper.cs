
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace reMCenters
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public static class Screens
    {
        public static MainWindow MainWindow { get; set; }
        static Frame MainContent { get; set; }
        static Grid FrontContent { get; set; }
        static CommonPage CommonPage { get; set; }
        public static Notification AddNotificationToQueue(string title, string message)=> CommonPage.AddNotificationToQueue(title, message);
        public static void ShowDialog(string title, string description, string cancelButtonText, string okButtonText, Action dialogOKAction, Action dialogCancelAction)=> CommonPage.ShowDialog(title,description,cancelButtonText, okButtonText, dialogOKAction,dialogCancelAction);
        public static void InitializeCommonPage(CommonPage commonPage)
        {

            CommonPage= commonPage;
            MainContent=CommonPage.MainContent;
            FrontContent = CommonPage.FrontContent;
        }
        
        public static void SetScreen(UIElement screen)
        {
            
            if(ReferenceEquals(screen,MainContent.Content)) return;
            
            MainContent.Content= screen;

        }
        public static UIElement GetScreen()=> MainContent.Content as UIElement;
        public static UIElement MainScreen { get; set; }
        public static UIElement SettingsScreen { get; set; }
        public static ErrorScreen ErrorScreen { get; set; }
        public static InstallScreen InstallScreen { get; set; }
        public static InstallScreen UninstallScreen { get; set; }
        public static ErrorScreen DllErrorScreen { get; set; }
        public static ModOptionsPage ModOptionsPage { get; set; }

    }
}
