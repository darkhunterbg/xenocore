using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CircleF : IEquatable<CircleF>
    {
        public Vector2 Position;
        public float Radius;

        public CircleF(Vector2 position, float radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        public CircleF(float x, float y, float r)
        {
            this.Position = new Vector2(x, y);
            this.Radius = r;
        }

        public void Intersects(ref Vector2 point, out bool result)
        {
            result = (point - Position).LengthSquared() <= Radius * Radius;
        }
        public bool Intersects(Vector2 point)
        {
            bool result = false;
            Intersects(ref point, out result);
            return result;
        }

        public void Intersects(ref CircleF circle, out bool result)
        {
            float l = (circle.Position - Position).LengthSquared();
            result = (l - Radius * Radius) <= circle.Radius * circle.Radius;
        }
        public bool Intersects(CircleF circle)
        {
            bool result = false;
            Intersects(ref circle, out result);
            return result;
        }
        public void Intersects(ref RectangleF rectangle, out bool result)
        {
            Vector2 tmp = Position;
            tmp.X -= Radius;

            rectangle.Contains(ref tmp, out result);
            if (result)
                return;

            tmp.X += 2 * Radius;
            tmp.Y -= Radius;

            rectangle.Contains(ref tmp, out result);
            if (result)
                return;

            tmp.X = Position.X;
            tmp.Y -= Radius;

            rectangle.Contains(ref tmp, out result);
            if (result)
                return;

            tmp.Y += 2 * Radius;

            rectangle.Contains(ref tmp, out result);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CircleF))
                return false;

            CircleF c = (CircleF)obj;
            return c.Position == Position && c.Radius == Radius;
        }
        public override int GetHashCode()
        {
            return Radius.GetHashCode() + Position.GetHashCode();
        }

        public bool Equals(CircleF other)
        {
            return Radius == other.Radius && other.Position == Position;
        }
    }
}
