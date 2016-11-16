using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Events;

namespace XenoCore.Engine.Scripting
{
    public class TimerExpiredEvent : Event<TimingSystem, uint>
    {
    }
}
