using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Editor
{
    public static class Extensions
    {
        public static String NormalizePath(this String s)
        {
            return  s.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar); 
        }
    }
}
