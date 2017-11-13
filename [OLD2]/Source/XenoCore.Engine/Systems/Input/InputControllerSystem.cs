using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Input;
using XenoCore.Engine.Systems.Events;

namespace XenoCore.Engine.Systems.Input
{
    public class InputControllerSystem
    {
        private readonly EventSystem EventSystem;
        private readonly InputService InputService;

        public List<InputBinding> Bindings { get; private set; } = new List<InputBinding>();

        public InputControllerSystem(SystemProvider systems)
            :this(systems.Get<EventSystem>())
        {
        }
        public InputControllerSystem(EventSystem es)
        {
            EventSystem = es;
            InputService = ServiceProvider.Get<InputService>();
        }

        public void UpdateInput()
        {
            foreach (InputBinding binding in Bindings)
            {
                bool trigger = false;

                if (binding.Key != Keys.None)
                {
                    switch (binding.Trigger)
                    {
                        case ButtonTrigger.Pressed:
                            trigger = InputService.State.WasKeyPressed(binding.Key);
                            break;
                        case ButtonTrigger.Released:
                            trigger = InputService.State.WasKeyReleased(binding.Key);
                            break;
                        case ButtonTrigger.Hold:
                            trigger = InputService.State.CurrentInput.Keyboard.IsKeyDown(binding.Key);
                            break;
                    }
                }

                if(trigger)
                {
                    EventSystem.EnqueueEvent<InputTriggeredEvent>(this, binding);
                }
            }
        }
    }
}
