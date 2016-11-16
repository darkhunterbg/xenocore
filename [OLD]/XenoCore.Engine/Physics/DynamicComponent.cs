using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;
using XenoCore.Engine.World;

namespace XenoCore.Engine.Physics
{

    [StructLayout(LayoutKind.Sequential)]
    public class DynamicComponent : Component
    {
        public Vector2 Velocity;
        public Vector2 Acceleration;

        internal Vector2 _simVelocity;
        internal Vector2 _simPosition;

        public float Drag;
        public bool IsAwake = true;

        internal float _energy;
        internal float _usedEnergy;

        public void Reset()
        {
            Velocity = Acceleration = Vector2.Zero;
            Drag = 0;
            IsAwake = true;


        }
    }
}
