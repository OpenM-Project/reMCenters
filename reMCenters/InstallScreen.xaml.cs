using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace reMCenters
{
    /// <summary>
    /// Interaction logic for InstallScreen.xaml
    /// </summary>
    /// 



    public partial class InstallScreen : Page
    {


        InstallScreenModeEnum mode = InstallScreenModeEnum.Start;
        public InstallScreenModeEnum Mode
        {
            get { return mode; }
            set
            {
                if (value == mode)
                    return;
                if (value == InstallScreenModeEnum.Uninstall)

                    TitleHead.Text = "Uninstall";
                else
                    TitleHead.Text = "Install";
                mode = value;




            }
        }
        public BindingExpression StartEnabled;
        readonly ColorBrushConverter converter;
        bool enabled = true;
        public bool ButtonEnabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
            }
        }
        double progressValue = 0;
        public double ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (this.progressValue == 100)
                {
                    ButtonEnabled = true;
                    StartEnabled.UpdateTarget();
                    Screens.MainWindow.EnableUninstall.UpdateTarget();

                }
                progressValue = value;

            }
        }
        public InstallScreen()
        {

            InitializeComponent();
            backButton.ConnectedImage = backIcon;
            startButton.ConnectedImage = startIcon;
            StartEnabled = startButton.GetBindingExpression(IsEnabledProperty);
            converter = new ColorBrushConverter();
            progressCaption.SetBinding(TextBlock.TextProperty, new Binding("Value") { Source = progressRing, Converter = converter });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {


            Screens.SetScreen(Screens.MainScreen);
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Status.Text = "Waiting to Start";
            ProgressValue = 0;
            if (Mode == InstallScreenModeEnum.Start)
            {
                ButtonEnabled = false;
                StartEnabled.UpdateTarget();
                Screens.MainWindow.EnableUninstall.UpdateTarget();

                await Install();

                ButtonEnabled = true;
                StartEnabled.UpdateTarget();
                Screens.MainWindow.EnableUninstall.UpdateTarget();
            }
            if (Mode == InstallScreenModeEnum.Uninstall)
            {
                ButtonEnabled = false;
                StartEnabled.UpdateTarget();
                Screens.MainWindow.EnableInstall.UpdateTarget();

                await UninstallDllMethod();

                ButtonEnabled = true;
                StartEnabled.UpdateTarget();
                Screens.MainWindow.EnableInstall.UpdateTarget();
            }

        }
        async Task UninstallDllMethod()
        {
            Methods.Method.ProgressChanged += DllMethod_ProgressChanged;
            var th = new Thread(Methods.DllMethod.UninstallMode) { Priority = ThreadPriority.Highest };
            th.SetApartmentState(ApartmentState.MTA);
            th.Start();
            while (th.ThreadState != ThreadState.Stopped)
                await Task.Delay(5000);

            Methods.Method.ProgressChanged -= DllMethod_ProgressChanged;
        }
        
       

        async Task InstallDllMethodOnline()
        {

            var th = new Thread(Methods.DllMethod.InstallDll) { Priority = ThreadPriority.Highest };
            Methods.Method.ProgressChanged += DllMethod_ProgressChanged;
            th.Start();
            while (th.ThreadState != ThreadState.Stopped)
                await Task.Delay(5000);



            
            
                var currentScreen = Screens.GetScreen();
                if (!ReferenceEquals(currentScreen, this))
                {
                    Screens.AddNotificationToQueue("Install DLL Method Online", Status.Text);
                }
            
            Methods.Method.ProgressChanged -= DllMethod_ProgressChanged;
        }



        async Task InstallDllMethodAutoPatch()
        {

            var th = new Thread(Methods.DllMethod.InstallDllAutoPatch) { Priority = ThreadPriority.Highest };
            Methods.Method.ProgressChanged += DllMethod_ProgressChanged;
            th.Start();
            while (th.ThreadState != ThreadState.Stopped)
                await Task.Delay(5000);





            var currentScreen = Screens.GetScreen();
            if (!ReferenceEquals(currentScreen, this))
            {
                Screens.AddNotificationToQueue("Install DLL Method Auto Patch", Status.Text);
            }

            Methods.Method.ProgressChanged -= DllMethod_ProgressChanged;
        }



        async Task Install()
        {
            var config = Config.Load();
            switch (config.SelectedMod)
            {
                case ModOptions.DllMethodOnline:
                    await InstallDllMethodOnline();
                    break;
                case ModOptions.DllMethodAutoPatch:
                    await InstallDllMethodAutoPatch();
                    break;
                default:
                    Screens.AddNotificationToQueue("Method not available", "Please use some other mod option");
                    break;
            }
        }

        private void DllMethod_ProgressChanged(object sender, Methods.ProgressEventArgs e)
        {
            this.Dispatcher.Invoke((
                ) =>
            {
                progressRing.Value = e.Progress;
                ProgressValue = e.Progress;
                Status.Text = e.Status;


            });

        }


    }
}
