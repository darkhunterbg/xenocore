using System;
using XenoCore.Engine.Threading;
using XenoCore.Desktop.Threading;
using System.IO;
using System.Threading;
using XenoCore.Engine.Logging;
using XenoCore.Desktop.Logging;
using XenoCore.Engine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using XenoCore.Engine.Input;
using XenoCore.Engine.Console;
using System.Linq;
using System.Reflection;
using XenoCore.Engine.Game;
using XenoCore.Engine.Screens;
using XenoCore.Desktop;

namespace XenoCore.Desktop.Test
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {


            DesktopPlatform.Initialize();

            using (var game = new XenoCoreDesktopGame())
            {
                game.Run();
            }

            DesktopPlatform.Uninitialize();
        }

    }
}
