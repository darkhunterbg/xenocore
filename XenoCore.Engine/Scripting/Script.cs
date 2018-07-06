using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Events;
using XenoCore.Engine.Screens;

namespace XenoCore.Engine.Scripting
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

    public class WaitTrigger
    {
        public Type EventType;
        public Object Sender;
        public Func<Event, bool> Condition;

        private WaitTrigger() { }

        public static WaitTrigger WaitForEvent<T>(Object sender = null, Func<T, bool> condition = null) where T : Event
        {
            var result = new WaitTrigger();
            if (condition != null)
                result.Condition = (Event e) => { return condition(e.Cast<T>()); };

            result.Sender = sender;
            result.EventType = typeof(T);

            return result;
        }
        public static WaitTrigger WaitTime(float seconds, SystemProvider systems)
        {
            uint timerid = systems.Get<TimingSystem>().NewTimer(seconds, false);
            return WaitForEvent<TimerExpiredEvent>(null, (e => e.Argument == timerid));
        }
    }
}
