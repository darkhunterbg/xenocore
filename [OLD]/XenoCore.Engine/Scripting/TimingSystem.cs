using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Events;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Scripting
{
    public class TimingSystem : ComponentSystem, IUpdateableSystem
    {
        private ComponentContainer<TimingComponent> components = new ComponentContainer<TimingComponent>(EntitySystem.MAX_ENTITIES / 1024);

        private ListArray<Timer> timers = new ListArray<Timer>(EntitySystem.MAX_ENTITIES / 4);
        private EventSystem eventSystem;


        public const int UPDATE_ORDER = UpdatingOrder.EVENTS - 50;

        public int UpdateOrder { get { return UPDATE_ORDER; } }
        public uint UpdatesPerSecond { get { return 60; } }

        public TimingSystem(EventSystem eventSystem, EntitySystem es)
           : base(es)
        {
            uint i = 0;
            foreach (var timer in timers)
                timer.ID = ++i;

            this.eventSystem = eventSystem;
        }

        public uint NewTimer(float duration, bool repeat)
        {
            var timer = timers.New();
            timer._remaining = timer.Duration = duration;
            timer.Repeat = repeat;

            return timer.ID;
        }

        public void Update(UpdateState updateState)
        {
            float deltaT = updateState.DeltaT;
            for (int i = 0; i < timers.Count; ++i)
            {
                var timer = timers[i];
                timer._remaining -= deltaT;
                while (timer._remaining <= 0)
                {
                    timer._component?.Callback();
                    eventSystem.EnqueueEvent<TimerExpiredEvent>(this, timer.ID);

                    if (timer.Repeat)
                        timer._remaining += timer.Duration;
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

        public TimingComponent AddComponent(uint entityid)
        {
            var component = components.New(entityid);
            component._timer = timers.New();
   
            EntitySystem.UnregisterComponentForEntity(this, component, entityid);

            return component;
        }

        public override void OnEntityDestroyed(Component systemComponent)
        {
            var c = systemComponent as TimingComponent;
            timers.Remove(c._timer);
            
            components.Remove(c);
        }
    }
}
