using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Events;

namespace XenoCore.Engine.Systems.Scripting
{
    public class ConditionContext
    {
        public Event TriggerEvent { get; internal set; }
        public SystemProvider Systems { get; internal set; }
    }
    public delegate bool Condition(ConditionContext context);

    public abstract class Script
    {
        internal IEnumerator<WaitTrigger> state;

        public Event TriggerEvent { get; internal set; }
        public SystemProvider Systems { get; internal set; }

        public abstract IEnumerator<WaitTrigger> Action();
        public abstract Trigger[] GetTriggers();
        public abstract Condition[] GetConditions();
    }

}
