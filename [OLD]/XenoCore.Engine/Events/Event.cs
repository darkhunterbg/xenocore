using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Events
{
    public interface IEventReciever
    {
        void OnEventDispatched(Event e);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Event
    {
        public Type EventType { get; internal set; }

        public Object Sender { get; internal set; }
        public Object Argument { get; internal set; }

        public T Cast<T>() where T : Event
        {
            return this as T;
        }
 
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Event<S, A> : Event
    {
        public new S Sender
        {
            get
            {
                return (S)base.Sender;
            }
            internal set
            {
                base.Sender = value;
            }
        }
        public new A Argument
        {
            get
            {
                return (A)base.Argument;
            }
            internal set
            {
                base.Argument = value;
            }
        }

    }

}
