using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Events;

namespace XenoCore.Engine.Systems.Input
{
    public enum ButtonTrigger
    {
        None = 0,
        Pressed = 1,
        Released = 2,
        Hold = 4
    }

    public class InputBinding
    {
        public Keys Key = Keys.None;

        public ButtonTrigger Trigger;

        public String CommandName { get; private set; }

        public InputBinding(String command)
        {
            this.CommandName = command;
        }
    }

    public class InputTriggeredEvent : Event<InputControllerSystem, InputBinding>
    {

    }
}
