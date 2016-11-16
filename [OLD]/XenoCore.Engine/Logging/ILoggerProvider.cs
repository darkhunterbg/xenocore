using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Logging
{
    public interface ILoggerProvider
    {
        Stream ConsoleStream { get; }
    }
}
