using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public abstract class Component
    {
        public Entity Entity { get; internal set; }

        internal ComponentSystem OwnedSystem { get; set; }
    }


    public class ComponentContainer<T> : IEnumerable<T> where T : Component
    {
        private ListArray<T> components;
        private Dictionary<uint, T> entityComponent;

        public ComponentContainer(int maxSize)
        {
            components = new ListArray<T>(maxSize);
            entityComponent = new Dictionary<uint, T>(maxSize);
        }

        public int Count
        {
            get { return components.Count; }
        }

        public T this[int index]
        {
            get
            {
                return components[index];
            }
        }

        public T GetComponent(int index)
        {
            return this[index];
        }


        public T GetByEntityId(uint id)
        {
            return entityComponent[id];
        }

        public T TryGetByEntityId(uint id)
        {
            T result;

            entityComponent.TryGetValue(id, out result);
            return result;
        }

        public T New(uint id)
        {
            T result = components.New();
            entityComponent.Add(id, result);
            
            return result;
        }
        public void Remove(T component)
        {
            components.Remove(component);
            entityComponent.Remove(component.Entity.ID);
        }
        public T Remove(uint entityId)
        {
            var component = entityComponent[entityId];
            Remove(component);
            return component;
        }
        public void RemoveAt(int index)
        {
            var component = components[index];
            components.RemoveAt(index);
            entityComponent.Remove(component.Entity.ID);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return entityComponent.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entityComponent.Values.GetEnumerator();
        }
    }
}
