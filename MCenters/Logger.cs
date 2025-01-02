using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCenters
{
    internal static class Logger
    {
        static  MCentersFileStream logWriter;
        public static  string  LogDirectory = "C:\\ProgramData\\reMCenters\\Logs\\";
        public static  string LogFileName = "log.txt";
        
        public static string LogPath { get; private set; }

        static bool IsInitialized = false;
        static void Initialize()
        {

            if (!Directory.Exists(LogDirectory)) Directory.CreateDirectory(LogDirectory);
            LogFileName = DateTime.Now.ToString("dddd_d_MMMM_yyyy hh_mm_ss_tt").Replace(':', '_') + ".txt";
            LogPath = LogDirectory + LogFileName;
            if(File.Exists(LogPath)) File.Delete(LogPath);
            logWriter = new MCentersFileStream(LogPath, FileMode.Append);
            IsInitialized = true;

        }
        
        public static void StartOperation(string OperationDescription)
        {

            var message = $"\n\n\n[{DateTime.Now}] Started Operation:\t\t{OperationDescription}\n\n";
            Write(message);

        }

        public static void CompleteOperation(string OperationDescription) {
            var message = $"\n\n[{DateTime.Now}] Completed Operation:\t\t{OperationDescription}\n\n";
            Write(message);

        }
        public static void Write(string message, bool IgnoreIfNotInitialized = false)
        {
            if (!IsInitialized && IgnoreIfNotInitialized) return;
            if (!IsInitialized) Initialize(); 
            
            logWriter.Write(Encoding.ASCII.GetBytes("\n"+message));
            logWriter.Flush();
        }
        public static void Close()
        {
            if (!IsInitialized) return;
            logWriter.Close();
        }
    }
}
