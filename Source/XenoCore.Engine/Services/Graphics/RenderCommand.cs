using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.InteropServices;

namespace XenoCore.Engine.Services.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public class RenderInstance
    {
        public Rectangle Destination;
        public Rectangle TexturePart;
        public Vector2 Center;
        public Vector2 TextScale;
        public Color Color;
        public String Text;
        public float Rotation;

        public SpriteEffects Effects;

        //No depth for now
        //public float Depth;
    }

    [StructLayout(LayoutKind.Sequential)]
    class RenderCommand
    {
        public UInt32 Key;
        public RenderCommandData Data = new RenderCommandData();
    }

    [StructLayout(LayoutKind.Sequential)]
    class RenderCommandData
    {
        public ListArray<RenderInstance> Instances { get; private set; } = new ListArray<RenderInstance>(4096);
        public bool IsSpriteFont;
    }
}
