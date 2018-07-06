using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Physics
{
    public enum ShapeType
    {
        Box,
        Circle
    }

    [StructLayout(LayoutKind.Sequential)]
    public abstract class Shape
    {
        public readonly Vector2 Offset;
        public readonly ShapeType Type;

        public float Restitution = 1.0f;

        public bool IsSensor = false;

        public Shape(Vector2 offset, ShapeType type)
        {
            Offset = offset;
            Type = type;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class BoxShape : Shape
    {
        internal RectangleF bb;
        public readonly Vector2 Size;

        public BoxShape(Vector2 size) : this(size, Vector2.Zero) { }

        public BoxShape(Vector2 size, Vector2 offset)
            : base(offset, ShapeType.Box)
        {
            this.Size = size;

            bb.Center = offset;
            bb.Inflate(size / 2);

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class CircleShape : Shape
    {
        public readonly float Radius;

        public CircleShape(float radius) : this(radius, Vector2.Zero) { }

        public CircleShape(float radius, Vector2 offset)
            : base(offset, ShapeType.Circle)
        {
            this.Radius = radius;

        }
    }
}
