using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Systems.Events
{
    public interface IEventHandler
    {
        void OnEvent(Event e);
    }

}
