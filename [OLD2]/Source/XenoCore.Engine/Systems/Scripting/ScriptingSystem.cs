using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Events;

namespace XenoCore.Engine.Systems.Scripting
{
    public class ScriptingSystem : IUpdateableSystem
    {
        private SystemProvider systems;

        private ListArray<ScriptDescription> allScripts = new ListArray<ScriptDescription>(128);
        private List<Event> enqueuedEvents = new List<Event>();
        private ConditionContext context = new ConditionContext();
        private List<Script> waitingScripts = new List<Script>();

        public int UpdateOrder { get { return UpdatingOrder.SCRIPTS; } }

        public ScriptingSystem(SystemProvider systems)
        {
            this.systems = systems;
        }



        public void LoadFromAssembly(String assemblyName, String @namespace = null)
        {
            LoadFromAssembly(Assembly.Load(new AssemblyName(assemblyName)), @namespace);
        }
        public void LoadFromAssembly(Assembly assembly, String @namespace = null)
        {
            List<Type> types = null;
            if (!String.IsNullOrEmpty(@namespace))
                types = assembly.ExportedTypes.Where(p => p.Namespace == @namespace).ToList();
            else
                types = assembly.ExportedTypes.ToList();

            var scriptTypes = types.Where(p => p.GetTypeInfo().BaseType == typeof(Script)).ToList();

            foreach (var type in scriptTypes)
            {
                var attribute = type.GetTypeInfo().CustomAttributes.FirstOrDefault(q => q.AttributeType == typeof(ScriptAttribute));
                Debug.AssertDebug(attribute != null, $"Script is missing ScriptAttribute! {type.FullName}");

                ScriptDescription descr = allScripts.NewInterlocked();

                var name = attribute.NamedArguments.Where(p => p.MemberName == "Name")
                        .Select(p => p.TypedValue.Value as String).FirstOrDefault();

                Debug.AssertDebug(!String.IsNullOrEmpty(name), $"ScriptAttribute.Name is empty! {type.FullName}");

                var enabledIterator = attribute.NamedArguments
                    .Where(p => p.MemberName == "Enabled")
                    .Select(p => (bool)p.TypedValue.Value);

                var enabled = enabledIterator.Count() > 0 ? enabledIterator.First() : true;

                var instance = Activator.CreateInstance(type) as Script;

                descr.Class = type;
                descr.Name = name;
                descr.Enabled = enabled;
                descr.Triggers = instance.GetTriggers();
                descr.Conditions = instance.GetConditions();
            }
        }

        public void EnqueueEvent(Event e)
        {
            enqueuedEvents.Add(e);
        }

        public void Update(UpdateState state)
        {
            foreach (Event e in enqueuedEvents)
            {
                context.TriggerEvent = e;

                for (int j = 0; j < waitingScripts.Count; ++j)
                {
                    var script = waitingScripts[j];
                    if (ShouldBeAwakened(script, e))
                    {
                        if (!script.state.MoveNext() || script.state.Current == null)
                        {
                            waitingScripts.RemoveAt(j);
                            --j;
                        }
                    }
                }

                for (int j = 0; j < allScripts.Count; ++j)
                {
                    ScriptDescription script = allScripts[j];
                    if (!script.Enabled)
                        continue;

                    if (ShouldBeTriggered(script, context))
                    {
                        Script instance = Activator.CreateInstance(script.Class) as Script;
                        instance.TriggerEvent = e;
                        instance.Systems = systems;
                        instance.state = instance.Action();

                        if (instance.state.MoveNext() && instance.state.Current != null)
                            waitingScripts.Add(instance);
                    }
                }
            }

            enqueuedEvents.Clear();
        }


        private bool ShouldBeAwakened(Script script, Event e)
        {
            WaitTrigger trigger = script.state.Current;
            if (trigger.EventType == e.EventType)
            {
                if (trigger.Sender == null || (trigger.Sender == e.Sender))
                {
                    return (trigger.Condition?.Invoke(e) ?? true);

                }
            }

            return false;
        }
        private bool ShouldBeTriggered(ScriptDescription script, ConditionContext context)
        {
            foreach (Trigger trigger in script.Triggers)
                if (trigger.EventType == context.TriggerEvent.EventType)
                {
                    foreach (var condition in script.Conditions)
                        if (!condition(context))
                            return false;

                    return true;
                }

            return false;
        }
    }
}
