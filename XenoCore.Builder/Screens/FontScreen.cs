using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.GUI;
using XenoCore.Engine.Screens;

namespace XenoCore.Builder.Screens
{
    class FontScreen : Screen
    {
        private Label label;

        public FontScreen()
        {
            Systems.Register(new GUISystem());

            var container = new StackContainer();
            container.Children.Add(label = new Label()
            {
                Text = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit.\nAenean commodo ligula eget dolor. Aenean massa.",
                TextHorizontalAlignment = HorizontalAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Center,
            });

            Systems.Get<GUISystem>().RootControl = container;
        }

        public void SetFont(Font font)
        {
            label.Font = font;
        }
    }
}
