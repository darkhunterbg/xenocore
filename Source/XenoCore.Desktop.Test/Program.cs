﻿using System;
using XenoCore.Engine;

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
        [STAThread]
        static void Main()
        {
            using (var game = new XenoCoreGame())
                game.Run();
        }
    }
}
