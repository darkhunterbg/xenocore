using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Events;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Scripting
{
    public class ScriptingSystem : IDisposable, IEventReciever, IUpdateableSystem
    {
        private ListArray<ScriptDescription> allScripts = new ListArray<ScriptDescription>(128);
        private List<Event> enqueuedEvents = new List<Event>(128);

        private List<Script> waitingScripts = new List<Script>();

        private EventSystem eventSystem;
        private SystemProvider systems;

        private ConditionContext context = new ConditionContext();

        public const int UPDATE_ORDER = UpdatingOrder.SCRIPTS - 50;

        public int UpdateOrder { get { return UPDATE_ORDER; } }
        public uint UpdatesPerSecond { get { return 60; } }

        public ScriptingSystem(EventSystem eventSystem, SystemProvider systems)
        {
            this.eventSystem = eventSystem;
            this.systems = systems;
            context.Systems = systems;
            eventSystem.AddAllEventsReciever(this);
            ConsoleService.LoadFromObject(this);
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
                if (attribute == null)
                    throw new Exception($"Script is missing ScriptAttribute! {type.FullName}");

                ScriptDescription descr = allScripts.NewInterlocked();

                var name = attribute.NamedArguments.Where(p => p.MemberName == "Name")
                        .Select(p => p.TypedValue.Value as String).FirstOrDefault();

                if (String.IsNullOrEmpty(name))
                    throw new Exception($"ScriptAttribute.Name is empty! {type.FullName}");

                var enabledIterator = attribute.NamedArguments.Where(p => p.MemberName == "Enabled")
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

        [ConsoleCommand(Name = "script_run", CompleterName = "script_name_completer")]
        public void RunScript(String scriptName)
        {
            for (int i = 0; i < allScripts.Count; ++i)
            {
                if (allScripts[i].Name == scriptName)
                {
                    var instance = Activator.CreateInstance(allScripts[i].Class) as Script;
                    instance.state = instance.Action();
                    instance.Systems = systems;

                    if (instance.state.MoveNext() && instance.state.Current != null)
                        waitingScripts.Add(instance);

                    return;
                }
            }


            ConsoleService.Warning($"No script found with name {scriptName}!");
        }
        [ConsoleCompleter(Name = "script_name_completer")]
        public List<String> ScriptNameCompleter(CompletionContext context)
        {
            return allScripts.Take(allScripts.Count).Select(p => p.Name).ToList();
        }

        public void Update(UpdateState updateState)
        {
            foreach(var e in enqueuedEvents)
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
                    var script = allScripts[j];
                    if (!script.Enabled)
                        continue;

                    if (ShouldBeTriggered(script, context))
                    {
                        var instance = Activator.CreateInstance(script.Class) as Script;
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
            var trigger = script.state.Current;
            if (trigger.EventType == e.EventType)
            {
                if (trigger.Sender == null
                    || (trigger.Sender == e.Sender))
                {
                    return (trigger.Condition?.Invoke(e) ?? true);

                }
            }

            return false;
        }
        private bool ShouldBeTriggered(ScriptDescription script, ConditionContext context)
        {
            foreach (var trigger in script.Triggers)
                if (trigger.EventType == context.TriggerEvent.EventType)
                {
                    foreach (var condition in script.Conditions)
                        if (!condition(context))
                            return false;

                    return true;
                }

            return false;
        }

        public void Dispose()
        {

            ConsoleService.UnloadFromObject(this);

            eventSystem.RemoveReceiverFromAllEvents(this);
        }

        public void OnEventDispatched(Event e)
        {
            enqueuedEvents.Add(e);
        }
    }
}
