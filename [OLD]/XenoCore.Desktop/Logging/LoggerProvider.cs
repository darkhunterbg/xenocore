using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Logging;

namespace XenoCore.Desktop.Logging
{
    public class LoggerProvider :  ILoggerProvider
    {
        private Stream stream;

        public Stream ConsoleStream
        {
            get
            {
                return stream;
            }
        }

        public LoggerProvider()
        {
            stream = Console.OpenStandardOutput();
            stream.Flush();
        }
    }
}
