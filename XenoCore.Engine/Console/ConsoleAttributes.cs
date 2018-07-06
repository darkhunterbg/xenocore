using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Console
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCommandAttribute : Attribute
    {
        public String Name { get; set; }
        public String CompleterName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleCompleterAttribute : Attribute
    {
        public String Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ConsoleCmdParam : Attribute
    {
        public String CompleterName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ConsoleVariableAttribute : Attribute
    {
        public String Name { get; set; }
        public String Default { get; set; }
        public String CompleterName { get; set; }
    }
}
