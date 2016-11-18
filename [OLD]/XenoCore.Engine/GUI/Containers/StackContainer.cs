using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;

namespace XenoCore.Engine.GUI
{

    public enum StackOrientation
    {
        Horizontal,
        Vertical
    }

    public class StackContainer : GUIContainer
    {
        public StackOrientation Orientation { get; set; }

        public override void Update(GUISystemState systemState)
        {
            if (Children.Count == 0)
                return;

            Rectangle space = State.Bounds;

            float totalWeight = GetTotalWeight();

            for (int i = 0; i < Children.Count; ++i)
            {
                var item = Children[i];

                Rectangle itemSpace = space;
                if (item.Weight > 0)
                {
                    if (Orientation == StackOrientation.Horizontal)
                        itemSpace.Width = (int)(space.Width * (item.Weight / totalWeight));
                    else
                        itemSpace.Height = (int)(space.Height * (item.Weight / totalWeight));

                    totalWeight -= item.Weight;
                }

                item.UpdateState(ref itemSpace);

                if (Orientation == StackOrientation.Horizontal)
                {
                    var takenSize = item.State.Bounds.Width + item.Margin.Width;
                    space.X += takenSize;
                    space.Width -= takenSize;
                }
                else
                {
                    var takenSize = item.State.Bounds.Height + item.Margin.Height;
                    space.Y += takenSize;
                    space.Height -= takenSize;
                }

                if (item.Visiblity != Visiblity.Visible)
                    continue;

                systemState.AddUpdateControl(this, item);
            }

        }
    }

}
