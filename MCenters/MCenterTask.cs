using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MCenters
{
    delegate string MCenterTaskStringBuilderDelegate(Exception exception);
    enum InvokeResults { busy, errorOccured, completed }

    internal class MCenterException : Exception {
    public MCenterException():base() { }
        public MCenterException(string message) : base(message) { }
    }
    internal class MCenterTask
    {
        private static Action CurrentlyRunningAction;
        private readonly Action Action;
        public MCenterTaskStringBuilderDelegate ErrorDescriptionBuilder { get; set; }
        public MCenterTaskStringBuilderDelegate ErrorTitleBuilder { get; set; }
        public MCenterTaskStringBuilderDelegate ErrorSubtitleBuilder { get; set; }


        public MCenterTask(Action newAction)
        {
            Action = newAction;

        }

        public static async Task<InvokeResults> CreateAndRunBasicOnThread(Action action)
        {
            var result = InvokeResults.busy;
            var th = new Thread(async () =>
            {
                var task = new MCenterTask(action);
            retry:;
                result = await task.Invoke();


                if (result == InvokeResults.busy) goto retry;
            });
              th.Start();
            while (th.ThreadState != ThreadState.Stopped)
                await Task.Delay(1000);
            return result;
        }
        public static InvokeResults CreateAndRunBasic(Action action,string ErrorDescription=null)
        {
            var result = InvokeResults.busy;
            
                var task = new MCenterTask(action) { ErrorDescriptionBuilder=(err)=>ErrorDescription };
            retry:;
                var invoker = task.Invoke();
            invoker.Wait();
            result = invoker.Result;

                if (result == InvokeResults.busy) goto retry;
            
            
            
                
            return result;
        }


        static bool IsMCentersLibraryPresentInvoker()
        {
            var action = new Action(() => MCentersLibrary.DllMethod.IsPresent());
            try
            {
                action();
                return true;
            }
            catch (FileNotFoundException err) when (err.HResult == -2147024770)
            {
                
                var is64 = Environment.Is64BitProcess;
                var str = is64 ? "x64" : "x86";
                var url = is64 ? "https://github.com/tinedpakgamer/M-Centers-8.0/raw/master/VC_redist.x64.exe" : "https://github.com/tinedpakgamer/M-Centers-8.0/raw/master/VC_redist.x86.exe";
                Application.Current.Dispatcher.Invoke(() =>
                {


                    Screens.ShowDialog("Microsoft Visual C++ Redistributable", $"Microsoft Visual C++ Redistributable {str} is required to use this Mod Option", "Later", "Download Now", () => Functions.OpenBrowser(url), null);
                });
                return false;

            }
            catch (Exception err) when (IsCriticalException(err))
            {
                
                Application.Current.Dispatcher.Invoke(() =>
                {


                    Screens.ShowDialog(err.GetType().FullName, $"{err.Message} {err.HResult}", "Cancel", "Contact Discord", () => Functions.OpenBrowser("https://discord.gg/sU8qSdP5wP"), null);
                });
                return false;
            }
            
        }
        public  static bool IsMCentersLibraryPresent()
        {
            return IsMCentersLibraryPresentInvoker();
            
        }
        

        public async Task<InvokeResults> Invoke(Type[] handledExceptionTypes = null, bool HandleAllExceptions = false, bool retryable = true)
        {

            if (CurrentlyRunningAction != null) return InvokeResults.busy;
            CurrentlyRunningAction = Action;


            if (handledExceptionTypes == null) handledExceptionTypes = new Type[0];

            retry:;
            try
            {
                Action();
                CurrentlyRunningAction = null;
                return InvokeResults.completed;

            }
            catch (Exception ex) when ((HandleAllExceptions && IsCriticalException(ex)) || handledExceptionTypes.Contains(ex.GetType()))
            {
                //this code is for exceptions handled by handledexceptiontypes
                bool? ShouldRetry = null;
                 Application.Current.Dispatcher.Invoke(async () =>
                {
                    Screens.ErrorScreen.ErrorTitle = ErrorTitleBuilder == null ? ex.HResult.ToString() : ErrorTitleBuilder(ex);
                    Screens.ErrorScreen.ErrorSubTitle = ErrorSubtitleBuilder == null ? ex.Message : ErrorSubtitleBuilder(ex);
                    Screens.ErrorScreen.ErrorDescription = ErrorDescriptionBuilder == null ? ex.StackTrace : ErrorDescriptionBuilder(ex);
                    Screens.ErrorScreen.copyButton.IsEnabled = true;
                    Screens.ErrorScreen.copyButton.Content = "Copy and Open Discord";

                    Screens.ErrorScreen.retryButton.Visibility = retryable ? Visibility.Visible : Visibility.Hidden;
                    


                    var innerEx = ex.InnerException ?? null;


                    var innerExceptionInfo = innerEx == null ? null : new { Type = innerEx.GetType().FullName, innerEx.HResult, innerEx.StackTrace, Message = innerEx.Message };
                    var innerExceptionInfoJson = innerExceptionInfo == null ? null :
    $@"{{
          ""Type"":    ""{innerExceptionInfo.Type}"",
       ""HResult"":     {innerExceptionInfo.HResult},
       ""Message"":    ""{innerExceptionInfo.Message}"",
    ""StackTrace"":    ""{innerExceptionInfo.StackTrace}""
                    
   }}
";
                    var exceptionInfo = new { Type = ex.GetType().FullName, ex.HResult, ex.StackTrace, Message=ex.Message };
                    var clipboardMessage =
    $@"```json
{{
          ""Type"":    ""{exceptionInfo.Type}"",
       ""HResult"":     {exceptionInfo.HResult},
       ""Message"":    ""{exceptionInfo.Message}""
    ""StackTrace"":    ""{exceptionInfo.StackTrace}"",
""InnerException"":    ""{innerExceptionInfoJson}""                
}}
```
";



                    Screens.ErrorScreen.CancelClicked += (s, eventE) =>
                    {
                        ShouldRetry = false;

                    };
                    Screens.ErrorScreen.RetryClicked += (s, eventE) =>
                    {
                        ShouldRetry = true;
                    };
                    Screens.ErrorScreen.CopyClicked += (s, eventE) =>
                    {

                        Clipboard.SetText(clipboardMessage);
                        Functions.OpenBrowser("https://discord.gg/sU8qSdP5wP");
                    };
                    var content = Screens.GetScreen();
                    Screens.SetScreen(Screens.ErrorScreen);
                    while (ShouldRetry == null) await Task.Delay(100);
                    Screens.SetScreen(content);
                    Screens.ErrorScreen.Reset();
                    if (ShouldRetry == true) return;
                    
                    CurrentlyRunningAction = null;
                }).Wait();
                if (ShouldRetry==true) goto retry;
                return InvokeResults.errorOccured;

            }
            catch (Exception ex) when (IsCriticalException(ex))
            {
                bool? ShouldRetry = null;
                 Application.Current.Dispatcher.Invoke(async () =>
                {
                    Screens.ErrorScreen.ErrorTitle = ex.HResult.ToString();
                    Screens.ErrorScreen.ErrorSubTitle = ex.Message;
                    Screens.ErrorScreen.ErrorDescription = ex.StackTrace;
                    Screens.ErrorScreen.copyButton.IsEnabled = true;
                    Screens.ErrorScreen.copyButton.Content = "Copy and Open Discord";

                    Screens.ErrorScreen.retryButton.Visibility = retryable ? Visibility.Visible : Visibility.Hidden;
                    
                    var innerEx = ex.InnerException ?? null;


                    var innerExceptionInfo = innerEx == null ? null : new { Type = innerEx.GetType().FullName, innerEx.HResult, innerEx.StackTrace, Message=innerEx.Message };
                    var innerExceptionInfoJson = innerExceptionInfo == null ? null :
    $@"{{
          ""Type"":    ""{innerExceptionInfo.Type}"",
       ""HResult"":    {innerExceptionInfo.HResult},
       ""Message"":    ""{innerExceptionInfo.Message}"",
    ""StackTrace"":    ""{innerExceptionInfo.StackTrace}""
                    
   }}
";
                    var exceptionInfo = new { Type = ex.GetType().FullName, ex.HResult, ex.StackTrace, Message=ex.Message };
                    var clipboardMessage =
    $@"```json
{{
          ""Type"":    ""{exceptionInfo.Type}"",
       ""HResult"":    {exceptionInfo.HResult},
       ""Message"":    ""{exceptionInfo.Message}""
    ""StackTrace"":    ""{exceptionInfo.StackTrace}"",
""InnerException"":    ""{innerExceptionInfoJson}""                
}}
```";
                    Screens.ErrorScreen.CancelClicked += (s, eventE) =>
                    {
                        ShouldRetry = false;

                    };
                    Screens.ErrorScreen.RetryClicked += (s, eventE) =>
                    {
                        ShouldRetry = true;
                    };
                    Screens.ErrorScreen.CopyClicked += (s, eventE) =>
                    {


                        Clipboard.SetText(clipboardMessage);
                        Screens.AddNotificationToQueue("Copied to Clipboard", "Error was copied to clipboard\nNow opening discord in browser");
                        Functions.OpenBrowser("https://discord.gg/sU8qSdP5wP");
                    };
                    var content = Screens.GetScreen();
                    Screens.SetScreen(Screens.ErrorScreen);
                    while (ShouldRetry == null)
                        await Task.Delay(100);
                    Screens.SetScreen(content);
                    Screens.ErrorScreen.Reset();
                    
                    if (ShouldRetry == true) {  return; };
                    CurrentlyRunningAction = null;
                }).Wait();
                if (ShouldRetry== true) goto retry;
                return InvokeResults.errorOccured;
            }
        }



        public static bool IsCriticalException(Exception ex)
        {
            // Check if the exception is of type SystemException or ApplicationException
            // You can customize this logic based on specific types you consider critical
            return ex is SystemException || ex is ApplicationException || ex is MCenterException;
        }

    }
}
