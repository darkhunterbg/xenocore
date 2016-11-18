using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RectangleF : IEquatable<RectangleF>
    {

        // Summary:
        //     Specifies the min of the rectangle.
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public Vector2 Min;
        //
        // Summary:
        //     Specifies the max of the rectangle.
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public Vector2 Max;
        //


        //
        // Summary:
        //     Initializes a new instance of Rectangle.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the rectangle.
        //
        //   y:
        //     The y-coordinate of the rectangle.
        //
        //   width:
        //     Width of the rectangle.
        //
        //   height:
        //     Height of the rectangle.
        public RectangleF(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        // Summary:
        //     Compares two rectangles for inequality.
        //
        // Parameters:
        //   a:
        //     Source rectangle.
        //
        //   b:
        //     Source rectangle.
        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !a.Equals(b);
        }
        //
        // Summary:
        //     Compares two rectangles for equality.
        //
        // Parameters:
        //   a:
        //     Source rectangle.
        //
        //   b:
        //     Source rectangle.
        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return a.Equals(b);
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() + Max.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is RectangleF)
                return Equals((RectangleF)obj);

            return false;
        }


        // Summary:
        //     Returns the y-coordinate of the bottom of the rectangle.
        public float Bottom { get { return Max.Y; } }


        public float X
        {
            get { return Min.X; }
            set {
                float delta = value - Min.X;
                Min.X= value;
                Max.X += delta;
            }
        }
        public float Y
        {
            get { return Min.Y; }
            set
            {
                float delta = value - Min.Y;
                Min.Y = value;
                Max.Y += delta;
            }
        }

        public Vector2 Size
        {
            get { return Max - Min; }
            set
            {
                var center = Center;
                Min = center - value / 2;
                Max = center + value / 2;
            }
        }

        public float Width
        {
            get { return Max.X - Min.X; }
            set { Max.X = value + Min.X; }
        }
        public float Height
        {
            get { return Max.Y - Min.Y; }
            set { Max.Y = value + Min.Y; }
        }

        //
        // Summary:
        //     Gets the Vector that specifies the center of the rectangle.
        public Vector2 Center
        {
            get { return (Max + Min) / 2; }
            set
            {
                var halfSize = (Max - Min) / 2.0f;
                Min = value - halfSize;
                Max = value + halfSize;
            }

        }
        //
        // Summary:
        //     Returns a Rectangle with all of its values set to zero.
        public static Rectangle Empty { get { return new Rectangle(); } }
        //
        // Summary:
        //     Gets a value that indicates whether the Rectangle is empty.
        public bool IsEmpty { get { return (Min + Max).LengthSquared() == 0; } }
        //
        // Summary:
        //     Returns the x-coordinate of the left side of the rectangle.
        public float Left { get { return Min.X; } }
        //
        // Summary:
        //     Returns the x-coordinate of the right side of the rectangle.
        public float Right { get { return Max.X; } }
        //
        // Summary:
        //     Returns the y-coordinate of the top of the rectangle.
        public float Top { get { return Min.Y; } }

        public bool Equals(RectangleF other)
        {
            return Min == other.Min && Max == other.Max;
        }

        // Summary:
        //     Determines whether this Rectangle contains a specified Vector.
        //
        // Parameters:
        //   value:
        //     The Point to evaluate.
        public bool Contains(Vector2 value)
        {
            return Min.X >= value.X && Max.X <= value.X &&
                Min.Y >= value.Y && Max.Y <= value.Y;
        }
        //
        // Summary:
        //     Determines whether this Rectangle entirely contains a specified Rectangle.
        //
        // Parameters:
        //   value:
        //     The Rectangle to evaluate.
        public bool Contains(RectangleF value)
        {
            return Min.X <= value.Min.X && Max.X >= value.Max.X &&
            Min.Y <= value.Min.Y && Max.Y >= value.Max.Y;
        }
        //
        // Summary:
        //     Determines whether this Rectangle contains a specified point represented
        //     by its x- and y-coordinates.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the specified point.
        //
        //   y:
        //     The y-coordinate of the specified point.
        public bool Contains(float x, float y)
        {
            return Min.X <= x && Max.X >= x &&
                Min.Y <= y && Max.Y >= y;
        }
        //
        // Summary:
        //     Determines whether this Rectangle contains a specified Point.
        //
        // Parameters:
        //   value:
        //     The Vector to evaluate.
        //
        //   result:
        //     [OutAttribute] true if the specified Vector is contained within this Rectangle;
        //     false otherwise.
        public void Contains(ref Vector2 value, out bool result)
        {
            result = Min.X >= value.X && Max.X <= value.X &&
                Min.Y >= value.Y && Max.Y <= value.Y;
        }
        //
        // Summary:
        //     Determines whether this Rectangle entirely contains a specified Rectangle.
        //
        // Parameters:
        //   value:
        //     The Rectangle to evaluate.
        //
        //   result:
        //     [OutAttribute] On exit, is true if this Rectangle entirely contains the specified
        //     Rectangle, or false if not.
        public void Contains(ref RectangleF value, out bool result)
        {
            result = Min.X <= value.Max.X && Max.X >= value.Min.X &&
            Min.Y <= value.Max.Y && Max.Y >= value.Min.Y;
        }

        //
        // Summary:
        //     Pushes the edges of the Rectangle out by the horizontal and vertical values
        //     specified.
        //
        // Parameters:
        //   infaltion:
        //     Value to push  out by.
        //
        public void Inflate(Vector2 infaltion)
        {
            Min -= infaltion;
            Max += infaltion;
        }
        //
        // Summary:
        //     Determines whether a specified Rectangle intersects with this Rectangle.
        //
        // Parameters:
        //   value:
        //     The Rectangle to evaluate.
        public bool Intersects(RectangleF value)
        {
            return Min.X <= value.Max.X && Max.X >= value.Min.X &&
            Min.Y <= value.Max.Y && Max.Y >= value.Min.Y;
        }
        //
        // Summary:
        //     Determines whether a specified Rectangle intersects with this Rectangle.
        //
        // Parameters:
        //   value:
        //     The Rectangle to evaluate
        //
        //   result:
        //     [OutAttribute] true if the specified Rectangle intersects with this one;
        //     false otherwise.
        public void Intersects(ref RectangleF value, out bool result)
        {
            result = Min.X <= value.Max.X && Max.X >= value.Min.X &&
           Min.Y <= value.Max.Y && Max.Y >= value.Min.Y;
        }
        //
        // Summary:
        //     Changes the position of the Rectangle.
        //
        // Parameters:
        //   amount:
        //     The values to adjust the position of the Rectangle by.
        public void Offset(Vector2 amount)
        {
            Min += amount;
            Max += amount;
        }
        //
        // Summary:
        //     Changes the position of the Rectangle.
        //
        // Parameters:
        //   offsetX:
        //     Change in the x-position.
        //
        //   offsetY:
        //     Change in the y-position.
        public void Offset(float offsetX, float offsetY)
        {
            Min.X += offsetX;
            Min.Y += offsetY;

            Max.X += offsetX;
            Max.Y += offsetY;
        }
        //
        // Summary:
        //     Retrieves a string representation of the current object.
        public override string ToString()
        {
            return String.Format("Min: {0}, Max: {1}", Min, Max);
        }
    }
}
