using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Desktop.Logging;
using XenoCore.Desktop.Threading;
using XenoCore.Engine.Input;
using XenoCore.Engine.Logging;
using XenoCore.Engine.Threading;

namespace XenoCore.Desktop
{
    public static class DesktopPlatform 
    {

        public static IThreadProvider ThreadProvider { get; private set; }
        public static ILoggerProvider LoggerProvider { get; private set; }

        static DesktopPlatform()
        {

        }

        public static void Initialize()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    ThreadProvider = new UnixThreadProvider();
                    break;
                case PlatformID.Win32NT:
                    ThreadProvider = new Win32ThreadProvider();
                    break;
                default:
                    throw new Exception($"Unsupported platform : {Environment.OSVersion.Platform}");
            }

            LoggerProvider = new LoggerProvider();
        }

        public static void Uninitialize()
        {
        }
    }
}
