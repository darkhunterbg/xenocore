using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public class RenderInstance
    {
        public Rectangle Destination;
        public Rectangle TexturePart;
        public Vector2 Center;
        public Color Color;
        public String Text;
        public float Rotation;
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
