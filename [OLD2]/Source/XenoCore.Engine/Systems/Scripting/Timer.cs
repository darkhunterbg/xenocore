using System;
using XenoCore.Engine.Systems.Events;

namespace XenoCore.Engine.Systems.Scripting
{
    class TimerInstance
    {
        public float Duration;
        public float Remaining;
        public bool Repeat;

        public Timer Timer;
    }

    public struct Timer : IEquatable<Timer>
    {
        internal int id;

        public bool IsEmpty
        {
            get { return id == 0; }
        }

        public Timer(int id)
        {
            this.id = id;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public bool Equals(Timer other)
        {
            return id == other.id;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Timer))
                return false;
            return obj.Equals((Timer)obj);
        }
    }

    public class TimerExpiredEvent : Event<TimingSystem, Timer>
    {
    }
}
