
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Utility
{
    public class Logger
    {
        public bool FileLogging { get; private set; }
        public string Name { get; private set; }
        public int ProcessId { get; private set; }
        public string LogHeader { get; private set; }
        public bool ConsoleLogging { get;private set; }
        private const string FileName = "rwa-log.txt";
        private const long MaxFileSize = 3*(long)1024*1024*1024;
        public Logger(bool fileLogging=false, bool consoleLogging= true)
        {
            Name = Process.GetCurrentProcess().ProcessName;
            ProcessId = Process.GetCurrentProcess().Id;
            LogHeader = Name + " " + ProcessId;
            FileLogging = fileLogging;
            ConsoleLogging = consoleLogging;
        }


        public void Log(string message)
        {
            var totalMessage = "<" + DateTime.Now + ">"+LogHeader + " " + message;
            if (FileLogging)
            {
                LogToFile(totalMessage);
            }
            if(ConsoleLogging)
            {
                Console.Out.WriteLine(totalMessage);
                Console.Out.Flush();
            }

        }

        private static void LogToFile(string message)
        {
            //We are writing to one file from different processes
            //So we have to handle synchronization with shared handle
            var waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "SHARED_BY_ALL_PROCESSES");
            waitHandle.WaitOne();
            if (!File.Exists(FileName))
            {
                using (var sw = File.CreateText(FileName))
                {
                    sw.WriteLine(message);
                }
            }

            if (new FileInfo(FileName).Length > MaxFileSize)
            {
                File.WriteAllText(FileName, string.Empty);
            }
            using (var sw = File.AppendText(FileName))
            {
                sw.WriteLine(message);
            }
            waitHandle.Set();
        }
    }
}
