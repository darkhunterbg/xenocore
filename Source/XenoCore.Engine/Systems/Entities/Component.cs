using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Systems.Entities
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
        private Dictionary<Entity, T> entityComponent;

        public ComponentContainer(int maxSize)
        {
            components = new ListArray<T>(maxSize);
            entityComponent = new Dictionary<Entity, T>(maxSize);
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


        public T GetByEntity(Entity entity)
        {
            return entityComponent[entity];
        }

        public T TryGet(Entity entity)
        {
            T result;

            entityComponent.TryGetValue(entity, out result);
            return result;
        }

        public T New(Entity entity)
        {
            T result = components.New();
            entityComponent.Add(entity, result);

            return result;
        }
        public void Remove(T component)
        {
            components.Remove(component);
            entityComponent.Remove(component.Entity);
        }
        public T Remove(Entity entity)
        {
            var component = entityComponent[entity];
            Remove(component);
            return component;
        }
        public void RemoveAt(int index)
        {
            var component = components[index];
            components.RemoveAt(index);
            entityComponent.Remove(component.Entity);
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
