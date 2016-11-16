using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;

namespace XenoCore.Engine.GUI
{
    public class Label : GUIControl
    {
        public Font Font { get; set; }
        public String Text { get; set; }

        public HorizontalAlignment TextHorizontalAlignment { get; set; }
        public VerticalAlignment TextVerticalAlignment { get; set; }

        public override void Update( GUISystemState systemState)
        {
            if (String.IsNullOrEmpty(Text) || Font.id ==0)
                return;

            var font = GraphicsService.Cache[Font];
            Vector2 textSize = font.MeasureString(Text);

            var instance = GraphicsService.Renderer.NewTextInterlocked(Font, 1, BlendingMode.Alpha);
            instance.Text = Text;
            instance.Rotation = 0;
            instance.Color = Color.White;
            instance.Center = Vector2.Zero;
            instance.Destination.X = State.Bounds.X;
            instance.Destination.Y = State.Bounds.Y;

            switch (TextHorizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    instance.Destination.X += (State.Bounds.Width - (int)textSize.X)/2;
                    break;
                case HorizontalAlignment.Right:
                    instance.Destination.X += (State.Bounds.Width - (int)textSize.X);
                    break;
            }

            switch (TextVerticalAlignment)
            {
                case VerticalAlignment.Center:
                    instance.Destination.Y += (State.Bounds.Height - (int)textSize.Y) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    instance.Destination.Y += (State.Bounds.Height - (int)textSize.Y);
                    break;
            }
        }
    }
}
