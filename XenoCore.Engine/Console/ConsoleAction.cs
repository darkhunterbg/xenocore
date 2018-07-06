using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Console
{
    public struct CompletionContext
    {
        public String ParameterText;
    }

    public delegate Object ConsoleVariableGetter();
    public delegate void ConsoleVariableSetter(Object var);

    public delegate void ConsoleCommandCallback(String[] parameters);
    public delegate List<String> ConsoleActionCompleter(CompletionContext context);

 

    class ConsoleAction
    {
        public String Name { get; set; }

        public bool IsVariable { get; set; }
        public String DefaultValue { get; set; }
        public bool IsVarArgs { get; set; }

        public ConsoleCommandCallback Callback { get; set; }
        public ConsoleActionCompleter[] Completers { get; set; }
    }
}
