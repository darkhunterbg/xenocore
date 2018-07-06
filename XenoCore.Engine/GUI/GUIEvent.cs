using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Input;

namespace XenoCore.Engine.GUI
{
    public enum GUIEventType
    {
        MouseClickStart,
        MouseClick,
        MouseHoverStart,
        MouseHoverEnd,
        MouseHover,
        ClickStart,
        Click,
    }

    public  class GUIEvent
    {
        public GUIEventType Type { get; private set; }

        public GUIEvent (GUIEventType type)
        {
            this.Type = type;
        }

        public T Cast<T>() where T : GUIEvent
        {
            return this as T;
        }
    }
    
    public class GUIMouseHoverEvent : GUIEvent
    {
        public Point Position;

        public GUIMouseHoverEvent() : base(GUIEventType.MouseHover) { }
        public GUIMouseHoverEvent(bool start) : base(start ? GUIEventType.MouseHoverStart : GUIEventType.MouseHoverEnd) { }
    }

    public class GUIMouseClickEvent : GUIEvent
    {
        public MouseButton Button;
        public Point Position;

        public GUIMouseClickEvent() : base(GUIEventType.MouseClick) { }
        public GUIMouseClickEvent(bool start) : base(start ? GUIEventType.MouseClickStart : GUIEventType.MouseClick) { }
    }
}
