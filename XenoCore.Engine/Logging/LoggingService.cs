using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;

namespace XenoCore.Engine.Logging
{
    public enum LogLevel
    {
        Info = 0x1,
        Debug = 0x2,
        Warning = 0x4,
        Error = 0x8,
    }


    public class LoggingService 
    {
        private static LoggingService instance;

        private Object mutex = new object();
        private StreamWriter consoleWriter;
        private LogLevel loggingMask = LogLevel.Info | LogLevel.Error | LogLevel.Warning | LogLevel.Debug;

        public static LogLevel LoggingMask
        {
            get { return instance.loggingMask; }
            set { instance.loggingMask = value; }
        }

        private LoggingService() { }

        public static void Initialize(ILoggerProvider loggerProvider)
        {
            instance = new LoggingService();

            if (loggerProvider.ConsoleStream != null)
            {
                instance.consoleWriter = new StreamWriter(loggerProvider.ConsoleStream);
                instance.consoleWriter.Flush();
            }
        }
        public static void Uninitialize()
        {
            lock (instance.mutex)
            {
                instance = null;
            }
        }

        public static void LogInfo(Object tag, Object obj)
        {
            Log(LogLevel.Info, tag, obj);
        }
        public static void LogError(Object tag, Object obj)
        {
            Log(LogLevel.Error, tag, obj);
        }
        public static void LogDebug(Object tag, Object obj)
        {
            Log(LogLevel.Debug, tag, obj);
        }
        public static void LogWarning(Object tag, Object obj)
        {
            Log(LogLevel.Warning, tag, obj);
        }
        public static void Log(LogLevel level, Object tag, Object obj)
        {
            if (LoggingMask.HasFlag(level))
            {
                instance.Log($"[{DateTime.Now:MM/dd/yyyy H::mm::ss} {level.ToString().ToUpper()}] ({tag}) {obj}");

                Color color = Color.White;
                switch(level)
                {
                    case LogLevel.Warning: color = Color.Yellow;break;
                    case LogLevel.Debug: color = Color.LightSkyBlue; break;
                    case LogLevel.Error: color = Color.Red; break;
                    case LogLevel.Info: color = Color.LightGray; break;
                }

                //Services.GetService<ConsoleService>()?.AddEntry($"[{tag}] {obj}", color);
            }
        }


        private  void Log(String data)
        {
            lock (mutex)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debug.WriteLine(data);

                consoleWriter?.WriteLine(data);
                consoleWriter?.Flush();
            }
        }
    }
}
