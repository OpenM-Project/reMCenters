using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SourceChord.FluentWPF;
using System;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;

namespace MCenters
{
    /// <summary>
    /// Interaction logic for Setting_Screen.xaml
    /// </summary>
    public partial class Setting_Screen : Page
    {
        public Setting_Screen()
        {
            InitializeComponent();
            backButton.ConnectedImage = backIcon;
            addButton.ConnectedImage = addIcon;
            viewButton.ConnectedImage = viewIcon;
            dllButton.ConnectedImage = dllIcon;
            logsFolderButton.ConnectedImage = logsFolderIcon;
            youtubeButton.ConnectedImage = youtubeIcon;
            discordButton.ConnectedImage = discordIcon;
            agreementButton.ConnectedImage = agreementIcon;
            policyButton.ConnectedImage = policyIcon;
            darkModeButton.ConnectedImage = darkModeIcon;
            thirdPartyBox.SetBinding(AcrylicPanel.IsEnabledProperty, new Binding("IsChecked") { Source = thirdPartyCheckBox });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {


            Screens.SetScreen(Screens.MainScreen);
        }



        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var provider = thirdPartyTextBox.Text.ToLower();
         var result=  await Methods.DllMethod.TryAddThirdParty(provider);
            thirdPartyManagementStatus.Text = result ? $"Successfully Added: {provider}" : $"Failed to Add: {provider}";
        }



        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {

        }



        private void DllButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Methods.DllMethod.baseDllPath);
            if(Directory.Exists(Methods.DllMethod.baseThirdPartyPath)) Process.Start("explorer.exe", Methods.DllMethod.baseThirdPartyPath);
        }



        private void ErrorButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Logger.LogDirectory);
        }


        


        private void YoutubeButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.OpenBrowser("https://www.youtube.com/@tinedpakgamer");
        }



        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.OpenBrowser("https://discord.gg/sU8qSdP5wP");
        }



        private void AgreementButton_Click(object sender, RoutedEventArgs e)
        {

        }



        private void PolicyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void thirdPartyCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DarkModeButton_Click(object sender, RoutedEventArgs e)
        {
            var config = Config.Load();
            config.IsDarkMode = !config.IsDarkMode;
            config.Save();
            
            UpdateTheme(config.IsDarkMode);
        }

        private void UpdateTheme(bool isDark)
        {
            var newTheme = new MaterialDesignThemes.Wpf.BundledTheme()
            {
                BaseTheme = isDark ? MaterialDesignThemes.Wpf.BaseTheme.Dark : MaterialDesignThemes.Wpf.BaseTheme.Light,
                PrimaryColor = MaterialDesignColors.PrimaryColor.DeepPurple,
                SecondaryColor = MaterialDesignColors.SecondaryColor.Blue
            };

            Application.Current.Resources.MergedDictionaries[1] = newTheme;
            Application.Current.Resources["MaterialDesignBackground"] = isDark ? Color.FromRgb(30, 30, 30) : Colors.White;
            Application.Current.Resources["GlobalTextForeground"] = new SolidColorBrush(isDark ? Colors.White : Colors.Black);
            Application.Current.Resources["ButtonBackground"] = new SolidColorBrush(Color.FromArgb(128, isDark ? (byte)40 : (byte)128, isDark ? (byte)40 : (byte)128, isDark ? (byte)40 : (byte)128));
            Application.Current.Resources["MaterialDesignTheme"] = isDark ? BaseTheme.Dark : BaseTheme.Light;

            darkModeButton.Content = isDark ? "Light Mode" : "Dark Mode";
            
            // Force refresh all controls
            foreach (Window window in Application.Current.Windows)
            {
                RefreshVisualChildren<TextBox>(window);
                RefreshVisualChildren<RichTextBox>(window);
                RefreshVisualChildren<Button>(window);
                RefreshVisualChildren<Border>(window);
            }
        }

        private void RefreshVisualChildren<T>(DependencyObject depObj) where T : FrameworkElement
        {
            foreach (var control in FindVisualChildren<T>(depObj))
            {
                var style = control.Style;
                control.Style = null;
                control.Style = style;
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                
                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }
}
