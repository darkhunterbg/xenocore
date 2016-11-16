using System;
using System.Linq;
using System.Collections.Generic;

namespace XenoCore.Engine.Systems.Entities
{
    public class Entity : IEquatable<Entity>
    {
        internal List<Component> _components = new List<Component>();
        internal bool _used;

        public int ID { get; internal set; }
        public IEnumerable<Component> Components { get { return _components; } }

        public T GetComponent<T>() where T : Component
        {
            return _components.FirstOrDefault(p => p.GetType() == typeof(T)) as T;
        }

        public bool Equals(Entity other)
        {
            return ID == other.ID;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
                return false;

            return ID == (obj as Entity).ID;
        }
        public override int GetHashCode()
        {
            return ID;
        }
    }
}
