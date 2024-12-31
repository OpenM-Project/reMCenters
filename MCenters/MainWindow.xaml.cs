using MaterialDesignThemes.Wpf;
using SourceChord.FluentWPF;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;

namespace MCenters
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow
    {
        public BindingExpression EnableUninstall;
        public BindingExpression EnableInstall;
        public readonly string CurrentVersion = "";
        public MainWindow()
        {            
            Screens.InstallScreen = new InstallScreen();
            Screens.UninstallScreen = new InstallScreen
            {
                Mode = InstallScreenModeEnum.Uninstall
            };
            var versionInfo = Process.GetCurrentProcess().MainModule.FileVersionInfo;
            CurrentVersion = versionInfo.FileVersion;
            ResourceDictionaryEx.GlobalTheme = ElementTheme.Light;
            InitializeComponent();
            Title = $"M Centers {versionInfo.FileMajorPart}.{versionInfo.FileMinorPart} Continued";
            
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Screens.SettingsScreen = new Setting_Screen();
            Screens.MainScreen = this.Content as UIElement;
            Screens.MainWindow = this;
            Screens.ErrorScreen = new ErrorScreen();
            Screens.DllErrorScreen = new ErrorScreen
            {
                CurrentMode = ErrorTypeEnum.ReportDll
            };
            Screens.ModOptionsPage = new ModOptionsPage();
            settingsButton.ConnectedImage = settingsLogo;
            installButton.ConnectedImage = installIcon;
            uninstallButton.ConnectedImage = uninstallIcon;
            modOptionsButton.ConnectedImage = modOptionsIcon;
            var commonPage=new CommonPage();
            this.Content = commonPage;

            Screens.InitializeCommonPage(commonPage);
            Screens.SetScreen(Screens.MainScreen);
            if (!Directory.Exists(Methods.Method.ClipboardFolder)) Directory.CreateDirectory(Methods.Method.ClipboardFolder);

            EnableUninstall = uninstallButton.GetBindingExpression(IsEnabledProperty);
            EnableInstall = installButton.GetBindingExpression(IsEnabledProperty);

            new Thread(CheckForUpdates).Start();
        }




        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Screens.SetScreen(Screens.SettingsScreen);
        }



        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            Screens.SetScreen(Screens.InstallScreen);
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            Screens.SetScreen(Screens.UninstallScreen);
        }

        private void AcrylicWindow_Closed(object sender, EventArgs e)
        {
            Logger.Write("\n\n\n\n\n\nApplication Closing",true);
            Logger.Close();
            Environment.Exit(0);
        }

        private void modOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Screens.SetScreen(Screens.ModOptionsPage);
        }

       void CheckForUpdates()
        {

            
            using(HttpClient client= new HttpClient())
            {

                try
                {
                    var invoker=client.GetAsync("https://raw.githubusercontent.com/misike12/M-Centers-8.0/master/CurrentVersion.txt");
                    invoker.Wait();
                    var result=invoker.Result;
                    if (!result.IsSuccessStatusCode) return;
                    var myVersion = new Version(Process.GetCurrentProcess().MainModule.FileVersionInfo.FileVersion);
                    var invoker2 = result.Content.ReadAsStringAsync();
                    invoker2.Wait();
                    var versionOnNet = new Version(invoker2.Result );
                    if(versionOnNet > myVersion)
                    {
                        Dispatcher.Invoke(() =>
                        {

                            Screens.ShowDialog("Update Available", $"Version {versionOnNet.ToString()} is now available for download.", "Later", "Download Now", () => Functions.OpenBrowser("https://github.com/misike12/M-Centers-8.0/releases"), null);
                        });

                    }
                    
                }
                catch(Exception err) when (MCenterTask.IsCriticalException(err))
                {

                }
            }
        }
    }
}
