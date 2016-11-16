using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    public static class CompilerSettings
    {
#if DEBUG
        public const bool DEBUG = true;
#else
        public const bool DEBUG = false;
#endif

#if DEV
        public const bool DEV = true;
#else
        public const bool DEV = false;
#endif
    }
}
