using System.Windows.Data;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace reMCenters
{
    
    public enum InstallScreenModeEnum { Start, Uninstall }
    public enum ErrorTypeEnum { Common, ReportDll, NetworkError, FileError };
    enum ErrorScreenResultEnum { pending, cancel, copy, retry }

    public static class Functions
    {
        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

       public static int FindStringPosition(byte[] byteArray, string searchString)
        {
            // Convert the search string to ASCII bytes
            byte[] searchBytes = Encoding.ASCII.GetBytes(searchString);

            // Iterate through the byte array to search for the string
            for (int i = 0; i <= byteArray.Length - searchBytes.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < searchBytes.Length; j++)
                {
                    if (byteArray[i + j] != searchBytes[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i; // Return position of the string
                }
            }
            return -1; // String not found
        }

    }
    public class ColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double progress = (double)value;
            if (progress == 0.0)
                return "";
            if (progress == 100.0)
                return "Completed";
            return ((int)progress).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    class reMCentersFileStream : FileStream
    {
        public reMCentersFileStream(string path, FileMode mode) : base(path, mode)
        {

        }

        public void Write(byte[] array)
        {
            base.Write(array, 0, array.Length);
        }
    }

}
