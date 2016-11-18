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

    class CallbackEventHandler<E> : IEventHandler where E: Event
    {
        public Action<E> Handler { get; private set; }

        public CallbackEventHandler(Action<E> handler)
        {
            Handler = handler;
        }

        public void OnEvent(Event e)
        {
            Handler.Invoke(e as E);
        }
    }


}
