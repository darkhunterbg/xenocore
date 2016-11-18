using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Graphics
{
    public struct Texture : IEquatable<Texture>
    {
        internal int id;

        public bool Equals(Texture other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            return obj is Texture && ((Texture)obj).id == id;
        }
        public override int GetHashCode()
        {
            return id;
        }
    }

    public struct Font
    {
        internal int id;
    }
}
