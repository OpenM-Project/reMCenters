using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SourceChord.FluentWPF;

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
    }
}
