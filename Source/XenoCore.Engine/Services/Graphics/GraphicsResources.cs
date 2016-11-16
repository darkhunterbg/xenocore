using System;

namespace XenoCore.Engine.Services.Graphics
{
    public struct Texture : IEquatable<Texture>
    {
        internal int id;

        public bool IsEmpty
        {
            get { return id == 0; }
        }

        public Texture(int id)
        {
            this.id = id;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public bool Equals(Texture other)
        {
            return id == other.id;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Texture))
                return false;
            return obj.Equals((Texture)obj);
        }
    }

    public struct Font : IEquatable<Font>
    {
        internal int id;

        public bool IsEmpty
        {
            get { return id == 0; }
        }

        public Font(int id)
        {
            this.id = id;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public bool Equals(Font other)
        {
            return id == other.id;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Font))
                return false;
            return obj.Equals((Font)obj);
        }
    }

}
