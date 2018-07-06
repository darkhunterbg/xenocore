using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct | AttributeTargets.Method)
]
    public class Debugger : System.Attribute
    {
        public String Name { get; set; }

        public Debugger(string name)
        {
            this.Name = name;
        }
    }

    public static class Debug
    {
        [Conditional("DEBUG")]
        public static void Assert(bool condition, String errorMsg)
        {
            if (!condition)
                throw new Exception($"Assertion failed! {errorMsg}");
        }
    }
}
