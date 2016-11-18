using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Console
{
    public static class DefaultCompleters
    {
        [ConsoleCompleter(Name = "boolean")]
        public static List<String> BooleanCompleter(CompletionContext context)
        {
            return new List<string>() { "true", "false" };
        }
    }
}
