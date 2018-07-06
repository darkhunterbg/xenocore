using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;

namespace XenoCore.Engine.Physics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SweptAABBResult
    {
        public Vector2 Normal;
        public float EntryTime;
        public float ExitTime;
    }

    public static class SweptTest
    {
        public static bool SweptAABBTest(ref RectangleF a, ref RectangleF b, ref Vector2 v, ref Vector2 bv, ref SweptAABBResult outResult)
        {
            Vector2 invEntry, invExit;

            if (v.X > 0)
            {
                invEntry.X = b.Left - a.Right;
                invExit.X = b.Right - a.Left;
            }
            else
            {
                invEntry.X = b.Right - a.Left;
                invExit.X = b.Left - a.Right;
            }
            if (v.Y > 0)
            {
                invEntry.Y = b.Top - a.Bottom;
                invExit.Y = b.Bottom - a.Top;
            }
            else
            {
                invEntry.Y = b.Bottom - a.Top;
                invExit.Y = b.Top - a.Bottom;
            }

            Vector2 entry = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 exit = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            if (v.LengthSquared() > 0)
            {
                if (v.X != 0.0f)
                {
                    entry.X = -invEntry.X / (v.X - bv.X);
                    exit.X = invExit.X / (v.X - bv.X);
                }
                if (v.Y != 0.0f)
                {
                    entry.Y = -invEntry.Y / (v.Y - bv.X);
                    exit.Y = invExit.Y / (v.Y - bv.Y);
                }
            }
            else
            {
                entry = Vector2.One;
                exit = Vector2.Zero;
            }

            outResult.EntryTime = 1.0f - Math.Min(entry.X, entry.Y);
            outResult.ExitTime = 1.0f - Math.Max(exit.X, exit.Y);

            bool collision = false;

            a.Intersects(ref b, out collision);
  
            if (outResult.EntryTime < 0.0001f && outResult.EntryTime > -0.0001f)
            {
                outResult.EntryTime = 0;
            }
            //if (outResult.EntryTime > 0.9999f )
            //    outResult.EntryTime = 1;

            //if (collision && (outResult.EntryTime < 0.0f || outResult.EntryTime > 1.0f))
            //{
            //    ConsoleService.Warning($"Collision detected with entry time {outResult.EntryTime}!");
            //    //  ConsoleService.ExecuteCommand("paused", "true");
            //}


            a.Intersects(ref b, out collision);

            if (collision && outResult.EntryTime >= 0.0f && outResult.EntryTime <= 1.0f)
            {
                if (entry.X < entry.Y)
                {
                    if (invEntry.X < 0.0f)
                        outResult.Normal = -Vector2.UnitX;
                    else
                        outResult.Normal = Vector2.UnitX;

                }
                else
                {
                    if (invEntry.Y < 0.0f)
                        outResult.Normal = -Vector2.UnitY;
                    else
                        outResult.Normal = Vector2.UnitY;
                }

                // ConsoleService.DevMsg($"Collision with entry time {outResult.EntryTime}");

                Debug.Assert(outResult.EntryTime >= 0.0f && outResult.EntryTime <= 1.0f, "Entry time is out of range!");
                Debug.Assert(outResult.Normal.LengthSquared() > 0.0f, "Normal vector is zero!");
            }
            else
                return false;

            return collision;
        }

    }
}
