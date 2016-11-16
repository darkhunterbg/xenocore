using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Events;

namespace XenoCore.Engine.Physics
{
    [StructLayout(LayoutKind.Sequential)]
    public class CollisionInfo
    {
        public Vector2 Normal;
        public Vector2 ColliderVelocity;

        public Shape ColliderShape;
        public Shape CollidesWithShape;

        public float CollisionTime;

        public uint ColliderID;
        public uint CollidesWithID;

        internal void LoadFromData(CollisionData data)
        {
            Normal = data.Normal;
            ColliderID = data.ColliderID;
            CollidesWithID = data.CollidesWithID;
            ColliderShape = data.ColliderShape;
            CollidesWithShape = data.CollidesWithShape;
        }
    }


    public class CollisionEvent : Event<PhysicsSystem, CollisionInfo>
    {
    }
}
