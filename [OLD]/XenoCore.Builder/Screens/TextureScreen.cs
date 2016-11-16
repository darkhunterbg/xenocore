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
    public class TextureScreen : Screen
    {

        private RelativeContainer container;
        private ImageBox imageBox;

        public TextureScreen(Texture? texture = null)
        {
            Systems.Register(new GUISystem());

            container = new RelativeContainer();
            container.Add(imageBox = new ImageBox()
            {
                Image = GraphicsService.Cache.GetTexture("White"),
            }, Vector2.Zero, Vector2.One);

            Systems.Get<GUISystem>().RootControl = container;

            if (texture != null)
                SetTexture(texture.Value);

        }

        public void SetTexture(Texture texture)
        {
            container.Remove(imageBox);
            imageBox.Image = texture;
            var raw = GraphicsService.Cache[texture];
            container.Add(imageBox, Vector2.Zero, new Vector2(raw.Width, raw.Height),
                 HorizontalAlignment.Center, VerticalAlignment.Center, true);
        }

    }
}
