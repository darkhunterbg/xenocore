using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;

namespace XenoCore.Engine.GUI
{

    public class AbsoluteContainer : GUIContainer
    {
        public override void Update(GUISystemState systemState)
        {
            foreach (var item in Children)
            {
                if (item.Visiblity != Visiblity.Visible)
                    continue;

                item.UpdateState( ref State.Bounds);

                systemState.AddUpdateControl(this, item);
            }
        }
    }
}
