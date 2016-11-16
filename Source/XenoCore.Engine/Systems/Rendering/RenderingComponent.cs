using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Graphics;
using XenoCore.Engine.Systems.Entities;

namespace XenoCore.Engine.Systems.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public class RenderingComponent : Component
    {
        public Vector2 Position;
        public Vector2 Scale;
        public Rectangle? TexturePart;
        public String Text;

        public Color Color;
        
        public float Rotation;
        public BlendingMode Blending;
        public bool FlipX;
        public bool FlipY;
        public bool IsVisible;

        public Texture Texture;
        public Font Font;

        public bool IsFont
        {
            get { return !Font.IsEmpty; }
        }

        internal void Reset()
        {
            Texture = new Texture();
            Font = new Font();
            Text = null;
            TexturePart = null;
            Color = Color.White;
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Rotation = 0;
            Blending = BlendingMode.Alpha;
            FlipX = FlipY = false;
            IsVisible = true;
            Text = String.Empty;
        }
    }

}
