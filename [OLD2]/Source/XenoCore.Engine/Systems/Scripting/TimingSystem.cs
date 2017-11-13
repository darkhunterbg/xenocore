using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Events;

namespace XenoCore.Engine.Systems.Scripting
{
    public class TimingSystem : IUpdateableSystem
    {
        private ListArray<TimerInstance> timers = new ListArray<TimerInstance>(128);
        private readonly EventSystem EventSystem;
        public int UpdateOrder { get {  return UpdatingOrder.TIMERS; } }

        public TimingSystem(SystemProvider systems) :
            this(systems.Get<EventSystem>())
        { }

        public TimingSystem(EventSystem eventSystem)
        {
            int i = 0;
            foreach (var instance in timers)
                instance.Timer = new Timer(++i);

            EventSystem = eventSystem;
        }

        public Timer NewTimer(float duration, bool repeat)
        {
            TimerInstance instance = timers.New();
            instance.Remaining = instance.Duration = duration;
            instance.Repeat = repeat;

            return instance.Timer;
        }

        public void Update(UpdateState updateState)
        {
            float deltaT = updateState.DeltaT;
            for (int i = 0; i < timers.Count; ++i)
            {
                TimerInstance instance = timers[i];
                instance.Remaining -= deltaT;
                while (instance.Remaining <= 0)
                {
                    EventSystem.EnqueueEvent<TimerExpiredEvent>(this, instance.Timer);

                    if (instance.Repeat)
                        instance.Remaining += instance.Duration;
                    else
                    {
                        timers.RemoveAt(i);
                        --i;
                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
        }

    }
}
