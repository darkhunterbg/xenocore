using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.GUI
{
    public class ControlState
    {
        internal bool MouseHovered { get;  set; }

        public Rectangle Bounds;
        public List<GUIEvent> Events { get; private set; } = new List<GUIEvent>();

        public GUIEvent RetrieveEvent(GUIEventType type)
        {
            for (int i = 0; i < Events.Count; ++i)
            {
                var e = Events[i];
                if (Events[i].Type == type)
                {
                    return e;
                }
            }


            return null;
        }
        public void ResolveEvent(GUIEvent e)
        {
            Events.Remove(e);
        }
    }

}
