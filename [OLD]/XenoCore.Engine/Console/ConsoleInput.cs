using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Console
{
    class ConsoleInput
    {
        public String Command { get; set; } = String.Empty;
        internal bool IsLastOK { get; set; }
        public List<String> Parameters { get; private set; } = new List<string>();
        public String LastParameter
        {
            get { return Parameters.LastOrDefault(); }
        }
        public int LastParameterIndex
        {
            get { return Parameters.Count - 1; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Command);
            foreach (var p in Parameters)
            {
                if (p.Contains(' '))
                    sb.Append($" \"{p}\"");
                else
                sb.Append($" {p}");
            }

            return sb.ToString();
        }

        public ConsoleInput()
        {
        }
        public ConsoleInput(String cmd, params String[] parameters)
        {
            Command = cmd;
            Parameters.AddRange(parameters);
        }
        public void Clear()
        {
            Command = String.Empty;
            Parameters.Clear();
            IsLastOK = false;
        }
    }

}
