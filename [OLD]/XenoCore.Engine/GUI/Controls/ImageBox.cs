using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;

namespace XenoCore.Engine.GUI
{
    public class ImageBox : GUIControl
    {
        public Texture Image { get; set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle? ImagePart;
        public float Rotation = 0;
        public BlendingMode Blending = BlendingMode.Alpha;

        public override void Update(GUISystemState systemState)
        {
            if (Image.id == 0)
                return;

            var instance = GraphicsService.Renderer.NewTextureInterlocked(Image, 1, Blending);
            instance.Center = Vector2.Zero;
            instance.Color = Color;

            var texture = GraphicsService.Cache[Image];

            if (ImagePart.HasValue)
                instance.TexturePart = ImagePart.Value;
            else
                instance.TexturePart = new Rectangle(0, 0, texture.Width, texture.Height);

            instance.Destination = State.Bounds;
            instance.Rotation = Rotation;
        }
    }
}
