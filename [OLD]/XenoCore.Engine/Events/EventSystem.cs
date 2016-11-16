using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Events
{
    public class EventSystem : IDisposable, IUpdateableSystem
    {
        //  private ListArray<Event> events = new ListArray<Event>(128);

        private Queue<Event> eventQueue = new Queue<Event>();
        private Dictionary<Type, List<EventRecieverData>> recievers = new Dictionary<Type, List<EventRecieverData>>();

        private List<IEventReciever> allEventsReciever = new List<IEventReciever>();

        public const int UPDATE_ORDER = UpdatingOrder.EVENTS;

        public int UpdateOrder { get { return UPDATE_ORDER; } }
        public uint UpdatesPerSecond { get { return 60; } }

        public void AddAllEventsReciever(IEventReciever reciever)
        {
            allEventsReciever.Add(reciever);
        }

        public void AddReciever<E>(IEventReciever reciever, Object onlyFromSender = null) where E : Event
        {
            var type = typeof(E);
            List<EventRecieverData> collection = null;
            if (!recievers.TryGetValue(type, out collection))
            {
                collection = new List<EventRecieverData>();
                recievers.Add(type, collection);
            }

            Debug.Assert(!collection.Any(p => p.Reciever == reciever), "Reciever is already registered!");
            collection.Add(new EventRecieverData() { Reciever = reciever, FromSender = onlyFromSender });
        }
        public void RemoveReciever<E>(IEventReciever reciever)
        {
            var collection = recievers[typeof(E)];
            for (int i = 0; i < collection.Count; ++i)
            {
                if (collection[i].Reciever == reciever)
                {
                    collection.RemoveAt(i);
                    return;
                }
            }

            Debug.Assert(false, "Reciever is not registered!");
        }
        public void RemoveReceiverFromAllEvents(IEventReciever reciever)
        {

            foreach (var collection in recievers.Values)
            {
                for (int i = 0; i < collection.Count; ++i)
                {
                    if (collection[i].Reciever == reciever)
                    {
                        collection.RemoveAt(i);
                        break;
                    }
                }
            }

            for (int i = 0; i < allEventsReciever.Count; ++i)
            {
                if (allEventsReciever[i] == reciever)
                {
                    allEventsReciever.RemoveAt(i);
                    break;
                }
            }
        }

        public void EnqueueEvent<T>(Object sender, Object args) where T : Event
        {
            var e = Activator.CreateInstance<T>();
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
                    foreach (var recieverData in collection)
                        recieverData.Reciever.OnEventDispatched(e);
                }

                foreach (var reciever in allEventsReciever)
                    reciever.OnEventDispatched(e);

            }

        }

        public void Dispose()
        {

        }
    }
}
