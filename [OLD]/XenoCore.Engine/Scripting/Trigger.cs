using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Scripting
{
    public class Trigger
    {
        public Type EventType { get; private set; }

        public Trigger(Type eventType)
        {
            EventType = eventType;
        }
    }
}
