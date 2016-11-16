using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Scripting
{
    [StructLayout(LayoutKind.Sequential)]
    class ScriptDescription
    {
        public String Name;
        public bool Enabled;
        public Type Class;

        public Trigger[] Triggers;
        public Condition[] Conditions;
    }
}
