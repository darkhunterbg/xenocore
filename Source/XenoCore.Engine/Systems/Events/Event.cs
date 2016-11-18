using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Systems.Events
{
    [StructLayout(LayoutKind.Sequential)]
    public abstract class Event
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
    public class Event<S, A> : Event where S : class where A : class
    {
        public new S Sender
        {
            get
            {
                return base.Sender as S;
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
                return base.Argument as A;
            }
            internal set
            {
                base.Argument = value;
            }
        }

    }
}
