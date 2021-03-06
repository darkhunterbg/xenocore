﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Physics
{
    [StructLayout(LayoutKind.Sequential)]
    public class CollisionData
    {
        public uint ColliderID;
        public uint CollidesWithID;
        public Shape ColliderShape;
        public Shape CollidesWithShape;
        public Vector2 Normal;
        public float EntryTime;
        public CollisionResolveAction Action;
    }
}
