using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Scripting;

namespace XenoCore.Engine.Systems.Events
{
    public class EventSystem : IUpdateableSystem
    {
        private Queue<Event> eventQueue = new Queue<Event>();
        private Dictionary<Type, List<IEventHandler>> recievers = new Dictionary<Type, List<IEventHandler>>();

        public int UpdateOrder { get { return 1; } }

        private readonly ScriptingSystem ScriptingSystem;

        public EventSystem(SystemProvider systems)
            : this(systems.Get<ScriptingSystem>())
        {

        }
        public EventSystem(ScriptingSystem ss)
        {
            ScriptingSystem = ss;
        }

        public void AddReciever<E>(IEventHandler reciever) where E : Event
        {
            var type = typeof(E);
            List<IEventHandler> collection = null;
            if (!recievers.TryGetValue(type, out collection))
            {
                collection = new List<IEventHandler>();
                recievers.Add(type, collection);
            }

            Debug.Assert(!collection.Contains(reciever), "Reciever is already registered!");
            collection.Add(reciever);
        }
        public void RemoveReciever<E>(IEventHandler reciever) where E : Event
        {
            var collection = recievers[typeof(E)];
            Debug.Assert(collection.Remove(reciever), "Reciever is not registered!");
        }

        public void EnqueueEvent<T>(Object sender, Object args) where T : Event
        {
            T e = Activator.CreateInstance<T>();
            e.Sender = sender;
            e.Argument = args;
            e.EventType = typeof(T);
            eventQueue.Enqueue(e);
        }

        public void Update(UpdateState state)
        {
            while (eventQueue.Count > 0)
            {
                var e = eventQueue.Dequeue();
                var key = e.EventType;

                if (recievers.ContainsKey(key))
                {
                    var collection = recievers[key];
                    foreach (var reciever in collection)
                        reciever.OnEvent(e);
                }

                ScriptingSystem.EnqueueEvent(e);
            }
        }
    }
}