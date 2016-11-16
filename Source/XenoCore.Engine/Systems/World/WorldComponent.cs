using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Entities;

namespace XenoCore.Engine.Systems.World
{
    [StructLayout(LayoutKind.Sequential)]
    public class WorldComponent : Component
    {
        public Vector2 Position;
        public Vector2 BaseSize;
        public Vector2 Scale = Vector2.One;

        public Vector2 ActualSize
        {
            get { return BaseSize * Scale; }
        }

        internal void Reset()
        {
            Position = BaseSize = Vector2.Zero;
            Scale = Vector2.One;
        }
    }
}
