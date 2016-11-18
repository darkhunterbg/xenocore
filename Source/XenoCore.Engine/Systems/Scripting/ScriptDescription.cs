using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Systems.Scripting
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptAttribute : Attribute
    {
        public String Name { get; set; }
        public bool Enabled { get; set; } = true;
    }

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
