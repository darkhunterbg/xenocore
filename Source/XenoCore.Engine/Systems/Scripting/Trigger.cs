using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Events;

namespace XenoCore.Engine.Systems.Scripting
{
    public class Trigger
    {
        public Type EventType { get; private set; }

        public Trigger(Type eventType)
        {
            EventType = eventType;
        }
    }


    public class WaitTrigger
    {
        internal Type EventType;
        internal Object Sender;
        internal Func<Event, bool> Condition;

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
        //public static WaitTrigger WaitTime(float seconds, SystemProvider systems)
        //{
        //    uint timerid = systems.Get<TimingSystem>().NewTimer(seconds, false);
        //    return WaitForEvent<TimerExpiredEvent>(null, (e => e.Argument == timerid));
        //}
    }
}
