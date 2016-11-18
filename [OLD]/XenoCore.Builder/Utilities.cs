using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Builder.MVVM;
using XenoCore.Engine.Particles;

namespace XenoCore.Builder
{
    public static class Utilities
    {
        public static String GetExecutablePath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), typeof(Utilities).Assembly.GetName().Name + ".exe");
        }
        public static String GetRelativePath(String fullPath)
        {
            Uri path1 = new Uri( fullPath);
            Uri path2 = new Uri(GetExecutablePath());
            Uri diff = path2.MakeRelativeUri(path1);
            return diff.OriginalString.Replace("/", @"\");
        }
    }
}
