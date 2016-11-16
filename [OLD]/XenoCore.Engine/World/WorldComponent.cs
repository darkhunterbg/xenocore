using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Graphics;

namespace XenoCore.Engine.World
{
    [StructLayout(LayoutKind.Sequential)]
    public class WorldComponent : Component
    {
        public Vector2 Position;
        public Vector2 BaseSize;
         
        public Vector2 Scale = Vector2.One;
        public Vector2 ParentOffset;
        public Color Color;
        public float Rotation = 0;
        public Texture Texture;

        public Vector2 ActualSize
        {
            get { return BaseSize * Scale; }
        }
        //   public Font? Font;
        //   public String Text;

        public BlendingMode Blending = BlendingMode.Alpha;

        public bool Render = true;

        internal void Reset()
        {
            Position = BaseSize = Vector2.Zero;
            Texture = new Texture();
            //    Font = null;

            Blending = BlendingMode.Alpha;
            Color = Color.White;
            //  Text = null;

            Scale = Vector2.One;

            ParentOffset = Vector2.Zero;

            Rotation = 0;
            Render = true;

        }
    }
}
