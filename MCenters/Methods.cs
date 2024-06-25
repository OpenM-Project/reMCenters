using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MCenters
{
       class Methods
    {
        public class ProgressEventArgs : EventArgs
        {
            public ProgressEventArgs(string status, double progress)
            {
                Status = status;
                Progress = progress;
            }
            public string Status { get; set; }
            public double Progress { get; set; }
        }
        public class Method

        {
            public static event EventHandler<ProgressEventArgs> ProgressChanged;
            static private protected void ReportProgress(string status, double progress)
            {
                ProgressChanged?.Invoke(null, new ProgressEventArgs(status, progress));
            }


            public static string ClipboardFolder = "C:\\ProgramData\\MCenters\\Clipboard\\";

            

            public static WebClient client = new WebClient();
            
            static public ErrorScreenResultEnum ShowIncompatibility(object identity)
            {
                throw new NotImplementedException();
            }

            










        }
        public class ExeMethod : Method
        {
            public static string baseExePath = "C:\\ProgramData\\MCenters\\Methods\\Exe";
            public ExeMethod(string version)
            {
                throw new NotImplementedException("Exe Method not implemented");
            }


        }
        public class DllMethod : Method
        {
            public static string Dllx64URL = "https://raw.githubusercontent.com/{username}/mcenterdlls/main/{identity}/x64/Windows.ApplicationModel.Store.dll";

            public static string Dllx86URL = "https://raw.githubusercontent.com/{username}/mcenterdlls/main/{identity}/x86/Windows.ApplicationModel.Store.dll";


            
            public static string baseDllPath = "C:\\ProgramData\\MCenters\\Methods\\Dll";
            public static string Dllx64AutoPatch = "C:\\ProgramData\\MCenters\\Methods\\AutoPatch\\x64\\Windows.ApplicationModel.Store.dll";
            public static string Dllx86AutoPatch = "C:\\ProgramData\\MCenters\\Methods\\AutoPatch\\x86\\Windows.ApplicationModel.Store.dll";
            public static string Dllx64AutoPatchDir = "C:\\ProgramData\\MCenters\\Methods\\AutoPatch\\x64";
            public static string Dllx86AutoPatchDir = "C:\\ProgramData\\MCenters\\Methods\\AutoPatch\\x86";
            public static string Dllx64 = "C:\\Windows\\System32\\Windows.ApplicationModel.Store.dll";
            public static string Dllx86 = "C:\\Windows\\SysWOW64\\Windows.ApplicationModel.Store.dll";
            public static string baseThirdPartyPath = "C:\\ProgramData\\MCenters\\Methods\\ThirdPartyDlls";

            public string SelectedProviderName = "tinedpakgamer";
            string SelectedPath = "";
            public static string[] AllThirdPartyUsernames = null;
            static DllMethod()
            {
                RefreshThirdPartyProviders();
            }
            static void RefreshThirdPartyProviders()
            {
                if (!Directory.Exists(baseThirdPartyPath)) return;
                var dirs = Directory.EnumerateDirectories(baseThirdPartyPath, "*", SearchOption.TopDirectoryOnly).ToArray();
                AllThirdPartyUsernames = dirs.Select((dir) => Path.GetFileName(dir)).ToArray();
            }
           public static async Task<bool> TryAddThirdParty(string provider)
            {
                
                

    var result=     await           MCenterTask.CreateAndRunBasicOnThread(()=> client.DownloadFile($"https://raw.githubusercontent.com/{provider}/mcenterdlls/main/main", "tmp.txt"));
                if (result == InvokeResults.completed)
                {
                    File.Delete("tmp.txt");
                    var providerPath=Path.Combine(baseThirdPartyPath, provider);
                    Directory.CreateDirectory(providerPath);
                    RefreshThirdPartyProviders();
                    return true;
                }
                return false;
                    
                

            }

            public string Version { get; set; }
            public bool IsDownloaded { get; private set; }
            public bool IsThirdPartyDownloaded { get; private set; }

           
            
            public static new ErrorScreenResultEnum ShowIncompatibility(object identity)
            {
                var systemDllVersion = identity is string ? identity as string : null;
                if (systemDllVersion == null) throw new MCenterException($"Invalid argument passed for ShowIncompatiblity: {systemDllVersion}");

                var response = ErrorScreenResultEnum.pending;

               Application.Current.Dispatcher.Invoke(()
          =>
                {
                    Screens.DllErrorScreen.CopyClicked += async (sender, e) => {
                        Screens.DllErrorScreen.copyButton.IsEnabled = false;
                        Screens.DllErrorScreen.retryButton.IsEnabled = false;
                        Screens.DllErrorScreen.cancelButton.IsEnabled = false;
                        response = await CopyDllsToClipBoard(systemDllVersion);
                        Screens.DllErrorScreen.copyButton.IsEnabled = true;
                        Screens.DllErrorScreen.retryButton.IsEnabled = true;
                        Screens.DllErrorScreen.cancelButton.IsEnabled = true;
                    };
                    Screens.DllErrorScreen.RetryClicked += (sender, e) => { response = ErrorScreenResultEnum.retry; };
                    Screens.DllErrorScreen.CancelClicked += (sender, e) => { response = ErrorScreenResultEnum.cancel; };
                    Screens.DllErrorScreen.ErrorTitle = "Unsupported Version " + systemDllVersion;
                    Screens.DllErrorScreen.ErrorSubTitle = "";
                    Screens.DllErrorScreen.ErrorDescription = "MCenters currently does not support your version of Windows.\nYou can retry, if you think it was a network issue.\nIf this is not a network issue then hit Submit Dlls to report your version to MCenters";
                    Screens.SetScreen(Screens.DllErrorScreen);

                });
                while (response == ErrorScreenResultEnum.pending)
                    Thread.Sleep(200);





                switch (response)
                {
                    case ErrorScreenResultEnum.retry:
                        Application.Current.Dispatcher.Invoke(()
       =>
                        {


                            Screens.SetScreen(Screens.InstallScreen);
                            ReportProgress("Waiting to Start", 0);
                            

                            Screens.DllErrorScreen.RemoveAllHandles();

                        });
                        break;

                    case ErrorScreenResultEnum.cancel:
                    case ErrorScreenResultEnum.copy:
                        Application.Current.Dispatcher.Invoke(()
      =>
                        {

                            Screens.SetScreen(Screens.MainScreen);
                            ReportProgress("Waiting to Start", 0);
                            Screens.DllErrorScreen.RemoveAllHandles();

                        });
                        break;



                }


                return response;

            }
            internal async static Task<ErrorScreenResultEnum> CopyDllsToClipBoard(string version)
            {

                StringCollection files = new StringCollection();

                var response = ErrorScreenResultEnum.pending;
                if (File.Exists(Dllx64))
                {
                    var fileName = $"{version} x64.dll";
                    fileName = Path.Combine(ClipboardFolder, fileName);


                    if (File.Exists(fileName))
                    {
                        var task = new MCenterTask(() => File.Delete(fileName))
                        {
                            ErrorDescriptionBuilder =
                                                     (ex) => { return $"An error occured while deleting ${fileName}"; }
                        };
                    retry:;

                        var result = await task.Invoke(
                                   new Type[] { typeof(IOException), typeof(UnauthorizedAccessException)
                                   });
                        while (result == InvokeResults.busy) goto retry;
                        if (result == InvokeResults.errorOccured) return response;

                    }
                    File.Copy(Dllx64, fileName);
                    files.Add(fileName);
                }

                if (File.Exists(Dllx86))
                {
                    var fileName = $"{version} x86.dll";
                    fileName = Path.Combine(ClipboardFolder, fileName);
                    if (File.Exists(fileName))
                    {

                        var task = new MCenterTask(() => File.Delete(fileName))
                        {
                            ErrorDescriptionBuilder =
                                                  (ex) => { return $"An error occured while deleting ${fileName}"; }
                        };
                    retry:;

                        var result = await task.Invoke(
                                   new Type[] { typeof(IOException), typeof(UnauthorizedAccessException)
                                   });
                        while (result == InvokeResults.busy) goto retry;
                        if (result == InvokeResults.errorOccured) return response;




                    }
                    File.Copy(Dllx86, fileName);
                    files.Add(fileName);
                }
                Clipboard.SetFileDropList(files);
                Screens.AddNotificationToQueue("Copied to Clipboard", "Files were copied to clipboard\nNow opening discord in browser");
                response = ErrorScreenResultEnum.copy;
                Functions.OpenBrowser("https://discord.gg/sU8qSdP5wP");
                return response;


            }

        

          


            public static void Uninstall()
            {
                ReportProgress("Fixing ClipSVC", 0);
                UninstallClipSVC();
                if (Environment.Is64BitProcess)
                    ReportProgress("Fixing x64 Dll", 25);
                else
                    ReportProgress("Fixing Dll", 50);
                SfcFileScan("C:\\Windows\\System32\\Windows.ApplicationModel.Store.dll");
                if (Environment.Is64BitProcess)
                {
                    ReportProgress("Fixing x86 Dll", 50);
                    SfcFileScan("C:\\Windows\\SysWOW64\\Windows.ApplicationModel.Store.dll");
                }
                ReportProgress("Uninstall Successful", 100);

      
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var restartAction = new Action(() => Process.Start(new ProcessStartInfo("shutdown", "/r /t 10 /c \"Restarting to uninstall Cracked Dlls\" /soft /d p:4:1")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }));

                    Screens.ShowDialog("Uninstall DLL Method", "A restart is required in order to finish uninstalling cracked DLLs", "Restart Later", "Restart Now", restartAction, null);
                });

            }


            static void UninstallClipSVC()
            {
                
                    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\ClipSVC\\Parameters", true);
                    registryKey.SetValue("ServiceDll", "%SystemRoot%\\System32\\ClipSVC.dll", RegistryValueKind.ExpandString);
                    registryKey.Close();
                
            }
            static void SfcFileScan(string fileName)
            {
              var  sfcPath = "C:\\Windows\\System32\\sfc.exe";

                Logger.StartOperation("Restoring " + fileName);
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(sfcPath, " /scanfile=" + fileName) { CreateNoWindow = true, UseShellExecute = false, RedirectStandardOutput = true, StandardOutputEncoding=Encoding.Unicode },
                    EnableRaisingEvents = true,
                                       
                };
                p.OutputDataReceived += (sender, e)=>
                {
                    if (string.IsNullOrWhiteSpace(e.Data)) return;
                    var asciiData = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(e.Data));
                    Logger.Write("Output Recieved:\t" + asciiData);
                };
               
                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
                Logger.CompleteOperation("Restoring " + fileName);
                

            }



            bool UseAutoPatch = false;


            public DllMethod(string DllVersion, string thirdpartyUserName=null,bool useAutoPatch=false)
            {
                UseAutoPatch=useAutoPatch;
                if (!useAutoPatch)
                {
                    SelectedProviderName = "tinedpakgamer";
                    SelectedPath = baseDllPath;
                    if (thirdpartyUserName != null && AllThirdPartyUsernames.Contains(thirdpartyUserName))
                    {
                        var path = Path.Combine(baseThirdPartyPath, thirdpartyUserName, "dll");
                        if (Directory.Exists(path))
                        {
                            SelectedPath = path;
                            SelectedProviderName = thirdpartyUserName;
                        }
                    }
                }
                ReportProgress("Starting Mod Installation " + DllVersion, 16.66);
                
                Version = DllVersion;
                if (useAutoPatch) return;
                if (Directory.Exists(SelectedPath))
                {
                    IsDownloaded = false;
                    var baseDirInfo = new DirectoryInfo(SelectedPath);
                    foreach (var dirInfo in baseDirInfo.EnumerateDirectories())
                    {
                        if (dirInfo.Name == DllVersion)
                        {

                            IsDownloaded = true;
                            break;
                        }
                    }

                }
                else
                {

                    Directory.CreateDirectory(SelectedPath);
                    IsDownloaded = false;

                }
            }

            public static bool IsAvailable(string version)
            {
                ReportProgress("Checking  Mod Support for " + version, 8.33);
                

                Logger.StartOperation("Is Dll mode available for " + version);
                if (Directory.Exists(baseDllPath))
                {

                    var baseDirInfo = new DirectoryInfo(baseDllPath);
                    foreach (var dirInfo in baseDirInfo.EnumerateDirectories())
                    {

                        if (dirInfo.Name == version)
                        {
                            Logger.Write("A downloaded version was found " + dirInfo.FullName);
                            Logger.CompleteOperation("Is Dll mode available for " + version);
                            
                            return true;

                        }
                    }

                }
                else
                    Directory.CreateDirectory(baseDllPath);

                Logger.Write("Checking Online");


                if (File.Exists(baseDllPath + "\\Records.txt"))

                    File.Delete(baseDllPath + "\\Records.txt");

                try
                {

                    client.DownloadFile("https://raw.githubusercontent.com/tinedpakgamer/mcenterdlls/main/main", baseDllPath + "\\Records.txt");

                    Logger.Write("File Downloaded");
                }
                catch (WebException)
                {
                    Logger.Write("Error Occured while downloading dll records");
                }


                var rawContent = File.ReadAllText(baseDllPath + "\\Records.txt");
                var rawContentCopy = rawContent;
                for (int index = 0; index < 10; ++index)
                    rawContentCopy = rawContentCopy.Replace(index.ToString(), "");
                rawContentCopy = rawContentCopy.Replace(".", "");
                rawContentCopy = rawContentCopy.Replace(",", "");
                var uselessCharArray = rawContentCopy.ToCharArray();
                foreach (var uselessChar in uselessCharArray)
                {

                    rawContent = rawContent.Replace(uselessChar.ToString(), "");
                }
                var Records = rawContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var entry in Records)
                {

                    if (entry == version)
                    {
                        Logger.Write("The requested version is available online");
                        Logger.CompleteOperation("Is Dll mode available for " + version);
                        
                        return true;
                    }
                }
                Logger.Write("Unsupported Version");
                Logger.CompleteOperation("Is Dll mode available for " + version);
                
                return false;


            }

            public static string IsAvailableOnThirdParty(string version)
            {
                if (AllThirdPartyUsernames == null) 
                    RefreshThirdPartyProviders();
                if (AllThirdPartyUsernames == null) return null;
                if(AllThirdPartyUsernames.Length==0) return null;
                ReportProgress("Checking  Mod Support for " + version+" via third party", 8.33);


                Logger.StartOperation("Is Dll mode available for " + version+" on third party");
                if (Directory.Exists(baseThirdPartyPath))
                {
                    foreach (var provider in AllThirdPartyUsernames)
                    {

                        var path = Path.Combine(baseThirdPartyPath, provider, "Dll");
                        if (!Directory.Exists(path)) continue;
                        var baseDirInfo = new DirectoryInfo(path);
                        foreach (var dirInfo in baseDirInfo.EnumerateDirectories())
                        {

                            if (dirInfo.Name == version)
                            {
                                Logger.Write($"A downloaded version was found by '{provider}' on '{dirInfo.FullName}'");
                                Logger.CompleteOperation("Is Dll mode available for " + version + " on third party");

                                return provider;

                            }
                        }
                    }
                    Logger.Write("Checking Online");
                    foreach (var provider in AllThirdPartyUsernames)
                    {

                        var path = Path.Combine(baseThirdPartyPath, provider, "Dll");
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        var recordsPath = Path.Combine(path, "Records.txt");
                        if (File.Exists(recordsPath)) File.Delete(recordsPath);
                        try
                        {

                            client.DownloadFile($"https://raw.githubusercontent.com/{provider}/mcenterdlls/main/main", recordsPath);

                            Logger.Write("Record.txt downloaded from "+provider);
                        }
                        catch (WebException)
                        {
                            Logger.Write("Error Occured while downloading dll records from "+provider);
                        }


                        var rawContent = File.ReadAllText(recordsPath);
                        var rawContentCopy = rawContent;
                        for (int index = 0; index < 10; ++index)
                            rawContentCopy = rawContentCopy.Replace(index.ToString(), "");
                        rawContentCopy = rawContentCopy.Replace(".", "");
                        rawContentCopy = rawContentCopy.Replace(",", "");
                        var uselessCharArray = rawContentCopy.ToCharArray();
                        foreach (var uselessChar in uselessCharArray)
                        {

                            rawContent = rawContent.Replace(uselessChar.ToString(), "");
                        }
                        var Records = rawContent.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (var entry in Records)
                        {

                            if (entry == version)
                            {
                                Logger.Write("The requested version is available online by" + provider);
                                Logger.CompleteOperation("Is Dll mode available for " + version + " on third party");

                                return provider;
                            }
                        }
                        
                       
                    }

                }
                Logger.Write("Unsupported Version");
                Logger.CompleteOperation("Is Dll mode available for " + version + " on third party");

                return null;

                


            }

            public static  string GetVersion()
            {

                ReportProgress("Fetching System Dll version", 0);
                
                

                Logger.StartOperation("Checking Version of System Dlls ");
                FileVersionInfo versionInfo;
                if (File.Exists(Dllx64))
                    versionInfo = FileVersionInfo.GetVersionInfo(Dllx64);
                else if (File.Exists(Dllx86))
                    versionInfo = FileVersionInfo.GetVersionInfo(Dllx86);
                else return "22000.348";
                var k = versionInfo.FileBuildPart.ToString() + "." + versionInfo.FilePrivatePart.ToString();
                Logger.Write("Detected " + k);
                Logger.CompleteOperation("Checking Version of System Dlls ");
                
                return k;

            }
          

            public  bool Download()
            {
                ReportProgress("Downloading " + Version, 25);
                Logger.StartOperation("Download of version " + Version);
                var versionPath = Path.Combine(SelectedPath, Version);
                if (Directory.Exists(versionPath))
                    Directory.Delete(versionPath);

                var x64DllFolder = Path.Combine(versionPath, "x64");
                var x86DllFolder = Path.Combine(versionPath, "x86");

                var x64DllFile = Path.Combine(x64DllFolder, "Windows.ApplicationModel.Store.dll");
                var x86DllFile = Path.Combine(x86DllFolder, "Windows.ApplicationModel.Store.dll");
                Directory.CreateDirectory(x64DllFolder);
                Directory.CreateDirectory(x86DllFolder);
                var x64DllDownloadUrl = Dllx64URL.Replace("{identity}", Version).Replace("{username}",SelectedProviderName);

                var x86DllDownloadUrl = Dllx86URL.Replace("{identity}", Version).Replace("{username}", SelectedProviderName); ;
                try
                {
                    client.Proxy = WebRequest.DefaultWebProxy;

                    client.DownloadFile(x64DllDownloadUrl, x64DllFile);
                    client.DownloadFile(x86DllDownloadUrl, x86DllFile);
                    Logger.Write("Files Downloaded");
                    Logger.CompleteOperation("Download of version " + Version);

                }
                catch (WebException)
                {
                    Logger.Write("Files failed to download");
                    Logger.CompleteOperation("Download of version " + Version);

                    return false;
                }

                return true;
            }


            private void TakePermissions(string path)
            {
                Logger.StartOperation("Taking Permissions of " + path);
                                    
                if (File.Exists(path))
                {
                    var executor = "takeown.exe";
                    var arguments= $"/f {path} /A";
                takeperms:;
                    var Info = new ProcessStartInfo(executor, arguments)
                    {
                        CreateNoWindow= true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        
                        Verb = "runas"
                    };
                    Process p = new Process
                    {
                        StartInfo = Info,
                        EnableRaisingEvents = true
                    };

                    p.OutputDataReceived += (sender, e)=>
                    {


                        if (String.IsNullOrWhiteSpace(e.Data))
                            return;

                        Logger.Write("Output Recieved:\t" + e.Data);
                    };
                    p.ErrorDataReceived += (sender,e)=>
                    {
                        if (String.IsNullOrWhiteSpace(e.Data))
                            return;

                        Logger.Write("Error Recieved:\t" + e.Data);

                    };

                    p.Start();
                    Logger.Write($"Process Started:\t{executor} {arguments}");
                    
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    if (p.ExitCode != 0)
                    {
                        throw new MCenterException($"Failed executing {executor} {arguments}\nProcess Exit Code: {p.ExitCode}");
                    }
                    if (executor == "takeown.exe")
                    {
                        executor = $"icacls.exe";
                        arguments = $"{path} /grant Administrators:F";
                        goto takeperms;
                    }
                    Logger.CompleteOperation("Taking Permissions of " + path);
                    



                }
            }

            private bool VerifyPermission(string path)
            {
                Logger.StartOperation("Verify Permissions of " + path);
                
                if (!File.Exists(path))
                    return true;

                var accessControl = new FileInfo(path).GetAccessControl();
                var fileOwner = accessControl.GetOwner(typeof(NTAccount)).Value;

                var userDomain = Environment.UserDomainName;
                var userName = Environment.UserName;
                if (fileOwner == "BUILTIN\\Administrators")
                {


                    Logger.Write("File is Owned");
                    Logger.CompleteOperation("Verify Permissions of " + path);
                    
                    return true;
                }

                Logger.Write("File is not Owned");
                Logger.CompleteOperation("Verify Permissions of " + path);

                return false;
            }
            bool Delete(string path)
            {

                Logger.StartOperation("Delete " + path);
                    Process[] p = Process.GetProcesses();
                    foreach (var item in p)
                    {

                        try
                        {
                            if (!item.HasExited)
                            {
                                foreach (var i in item.Modules)
                                {
                                    ProcessModule o = (ProcessModule)i;
                                    if (o.FileName == path)
                                    {
                                        Logger.Write("Ended Process " + item.ProcessName);
                                        
                                        item.Kill();
                                    }
                                }
                            }
                        }
                        catch (System.ComponentModel.Win32Exception)
                        {
                            Logger.Write("Unable to end Process " + item.ProcessName);
                            
                        }
                    }
                    Logger.Write("Process Ending Completed");
                    
                    ;
                    File.Delete(path);

                    Logger.Write("File Deleted");
                    Logger.CompleteOperation("Delete " + path);
                    return true;
            }
            void Replace(string Path, bool Is64)
            {


                Logger.StartOperation("Replace file " + Path + "   with x64 Mode: " + Is64);
                var sourcePath = "";
                var autopatchmsg = UseAutoPatch ? " with Auto Patch" : "";
                if (Is64)
                
                    sourcePath = UseAutoPatch ? Dllx64AutoPatch : SelectedPath + "\\" + Version + "/x64/Windows.ApplicationModel.Store.dll";

                else
                    sourcePath = UseAutoPatch ? Dllx86AutoPatch : SelectedPath + "\\" + Version + "/x86/Windows.ApplicationModel.Store.dll";
                File.Copy(sourcePath, Path);

                Logger.Write("File Replaced");
                Logger.CompleteOperation("Replace file " + Path + "   with x64 Mode: " + Is64+autopatchmsg);
            }

           
            
            
            public  bool Install()
            {
                int i = 0;
                bool Is64 = Environment.Is64BitProcess;
                if (IsDownloaded)
                    i = -1;
                ReportProgress("Taking permission from System32", Is64 ? ((4.0 + i) / 12.0) * 100 : ((4 + i) / 8) * 100);
                TakePermissions(Dllx64);
                ReportProgress("Verifying permission from System32", Is64 ? ((5.0 + i) / 12.0) * 100 : ((5 + i) / 8) * 100);
                VerifyPermission(Dllx64);
                ReportProgress("Deleting a file from System32", Is64 ? ((6.0 + i) / 12.0) * 100 : ((6 + i) / 8) * 100);
                Delete(Dllx64);
                ReportProgress("Replacing a file in System32", Is64 ? ((7.0 + i) / 12.0) * 100 : ((7 + i) / 8) * 100);
                Replace(Dllx64, Environment.Is64BitProcess);

                if (Environment.Is64BitProcess)
                {
                    ReportProgress("Taking permission from SysWoW64", ((8.0 + i) / 12.0) * 100);
                    TakePermissions(Dllx86);
                    ReportProgress("Verifying permission from SysWoW64", ((9.0 + i) / 12.0) * 100);
                    VerifyPermission(Dllx86);
                    ReportProgress("Deleting a file from SysWoW64", ((10.0 + i) / 12.0) * 100);
                    Delete(Dllx86);
                    ReportProgress("Replacing a file in SysWoW64", ((11.0 + i) / 12.0) * 100);
                    Replace(Dllx86, false);

                }
                ReportProgress("Mod Installed", 100);
                return true;
            }



           

        public static  void InstallDll()
            {


                
            retry:;
                var systemDllVersion = "";

                var systemDllVersionCheckTaskResult = MCenterTask.CreateAndRunBasic(() => { systemDllVersion = Methods.DllMethod.GetVersion(); }, "An error occured while fetching system dll version");
                if (systemDllVersionCheckTaskResult == InvokeResults.errorOccured)
                {
                    ReportProgress("Error occured last time while checking system dll version",0);


                    return;
                }





                bool isVersionAvailable = false;
                string provider = null;


                var DllMethodAvailabitityCheckerTaskResult = MCenterTask.CreateAndRunBasic(() => {
                    isVersionAvailable = Methods.DllMethod.IsAvailable(systemDllVersion);

                    if (!isVersionAvailable)
                    {
                        provider = Methods.DllMethod.IsAvailableOnThirdParty(systemDllVersion);
                    }
                }, "An error occured while checking version availability");
                if (DllMethodAvailabitityCheckerTaskResult == InvokeResults.errorOccured)
                {
                    
                    ReportProgress("Error occured last time while checking crack support for system dll",0);

                    return;
                }

                if (isVersionAvailable || provider != null)
                {
                    var method = new Methods.DllMethod(systemDllVersion, provider);
                    if (!method.IsDownloaded)
                    {



                        var DllMethodDownloaderTaskResult = MCenterTask.CreateAndRunBasic(() => method.Download(), "An error occured while download Dll method");
                        if (DllMethodDownloaderTaskResult == InvokeResults.errorOccured)
                        {
                            
                                ReportProgress( "Error occured last time while downloading cracked dll",0);

                            return;
                        }




                    }




                    var dllMethodInstallTaskResult = MCenterTask.CreateAndRunBasic(() => method.Install(), "An error occured while installing dll method");

                    if (dllMethodInstallTaskResult == InvokeResults.errorOccured)
                    {
                        
                            ReportProgress("Error occured last time while installing dll method",0);


                        return;
                    }


                }
                else
                {
                    var response = Methods.DllMethod.ShowIncompatibility(systemDllVersion);
                    if (response == ErrorScreenResultEnum.retry) goto retry;

                }
            

            }




            bool DoAutoPatch(bool is64)
            {
                if(!Directory.Exists(Dllx64AutoPatchDir) && is64) Directory.CreateDirectory(Dllx64AutoPatchDir);
                if (!Directory.Exists(Dllx86AutoPatchDir)) Directory.CreateDirectory(Dllx86AutoPatchDir);

                if (File.Exists(Dllx64AutoPatch)) File.Delete(Dllx64AutoPatch);
                if (File.Exists(Dllx86AutoPatch)) File.Delete(Dllx86AutoPatch);
                File.Copy(Dllx64, is64 ? Dllx64AutoPatch : Dllx86AutoPatch);
                if (is64) File.Copy(Dllx86, Dllx86AutoPatch);
                if (is64) { 
                    if(!MCentersLibrary.DllMethod.Patchx64Dll()) return false; }
              return  MCentersLibrary.DllMethod.Patchx86Dll();
            }

            public static void InstallDllAutoPatch()
            {
                bool is64 = Environment.Is64BitProcess;

                if (!MCenterTask.IsMCentersLibraryPresent()) return;
                



            retry:;
                var systemDllVersion = "";

                var systemDllVersionCheckTaskResult = MCenterTask.CreateAndRunBasic(() => { systemDllVersion = GetVersion(); }, "An error occured while fetching system dll version");
                if (systemDllVersionCheckTaskResult == InvokeResults.errorOccured)
                {
                    ReportProgress("Error occured last time while checking system dll version", 0);


                    return;
                }





                
                bool isPlatformSupported = false;

                var DllMethodAvailabitityCheckerTaskResult = MCenterTask.CreateAndRunBasic(() => {
                    var platform = 0;
                    
                        platform = MCentersLibrary.DllMethod.GetPlatform(Dllx64);
                    
                    
                    if (platform == 0 || (is64 && platform == 2)) return;
                    if (!is64)
                    {
                        isPlatformSupported=true;
                        return;
                    }
                    platform = MCentersLibrary.DllMethod.GetPlatform(Dllx86);
                    if(platform!=2) return;
                    isPlatformSupported = true;
                    return;
                   
                }, "An error occured while checking platform of DLLs");
                if (DllMethodAvailabitityCheckerTaskResult == InvokeResults.errorOccured)
                {

                    ReportProgress("Error occured last time while checking platform of DLLs", 0);

                    return;
                }

                if (isPlatformSupported)
                {
                    var method = new Methods.DllMethod(systemDllVersion,null,true);

                    bool wasPatchSuccess = false;

                        var DllMethodDownloaderTaskResult = MCenterTask.CreateAndRunBasic(() =>wasPatchSuccess= method.DoAutoPatch(is64), "An error occured while patching DLLs");
                        if (DllMethodDownloaderTaskResult == InvokeResults.errorOccured|| wasPatchSuccess==false)
                        {

                            ReportProgress("Error occured last time while creating cracked dll", 0);

                            return;
                        }




                    




                    var dllMethodInstallTaskResult = MCenterTask.CreateAndRunBasic(() => method.Install(), "An error occured while installing dll method");

                    if (dllMethodInstallTaskResult == InvokeResults.errorOccured)
                    {

                        ReportProgress("Error occured last time while installing dll method", 0);


                        return;
                    }


                }
                else
                {
                    var response = Methods.DllMethod.ShowIncompatibility(systemDllVersion);
                    if (response == ErrorScreenResultEnum.retry) goto retry;

                }


            }


            public static  void UninstallMode()
            {

               
              var  result = MCenterTask.CreateAndRunBasic(() => Uninstall(), "An error occured while uninstalling Mod");

                if (result == InvokeResults.errorOccured)
                {
                    
                    ReportProgress("Error occured last time while uninstalling dll method", 0);
                };


            }


        }
    }
}
