using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    public static class Extensions
    {
        public static float NextFloat(this Random r, float min, float max)
        {
            float i = (float)r.NextDouble();
            i *= (max - min);
            return min + i;
        }
        public static byte NextByte(this Random r)
        {
           return (byte)r.Next(0, Byte.MaxValue);
        }

        public static RectangleF ToRectangleF(this Rectangle r)
        {
            return new RectangleF()
            {
                X = r.X,
                Y =r.Y,
                Width = r.Width,
                Height = r.Height
            };
        }

        public static Vector2 Rotate(this Vector2 v, float angleRad)
        {
            float sin = (float)Math.Sin(angleRad);
            float cos = (float)Math.Cos(angleRad);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
            return v;
        }

        public static Color Multiply(this Color c1, Color c2)
        {
            Vector4 color = c1.ToVector4();
            color *= c2.ToVector4();

            return new Color(color);
        }
    }
}
