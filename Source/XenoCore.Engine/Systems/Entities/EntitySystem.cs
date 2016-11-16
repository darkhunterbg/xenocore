using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Systems.Entities
{
    public class EntitySystem
    {
        public int MaxEntites { get; private set; }

        private Entity[] entities;
        private Queue<Entity> recycled = new Queue<Entity>();

        private int lastid = 0;

        public IEnumerable<Entity> Entities
        {
            get { return entities.Where(p => p._used); }
        }

        public EntitySystem(int maxEntites)
        {
            this.MaxEntites = maxEntites;

            entities = new Entity[maxEntites];

            for (int i = 0; i < entities.Length; ++i)
            {
                entities[i] = new Entity()
                {
                    ID = i,
                };
            }
        }

        public Entity NewEntity()
        {
            Entity entity;

            if (recycled.Count == 0)
                entity  =  entities[++lastid];
            else
               entity = recycled.Dequeue();

            entity._used = true;

            return entity;
        }
        [Conditional("DEBUG")]
        private void ValidEntityCheck(Entity entity, String msg)
        {
            Debug.AssertDebug(entity.ID <= lastid && !recycled.Contains(entity), msg) ;
        }

        public void DeleteEntity(Entity entity)
        {
            ValidEntityCheck(entity, "Invalid entity to delete!");

            foreach (var component in entity.Components)
                component.OwnedSystem.OnEntityDestroyed(component);

            entity._components.Clear();
            entity._used = false;

            recycled.Enqueue(entity);
        }

        public void RegisterComponentForEntity(ComponentSystem system, Component component, Entity entity)
        {
            Debug.AssertDebug(system != null, "Component system cannot be null!");
            ValidEntityCheck(entity, "Invalid entity to add component for!");
            Debug.AssertDebug(component.Entity == null, "Component is already added to another entity!");
            Debug.AssertDebug(entity._components.All(p=>p.GetType()!= component.GetType()), "Component of the same type is already added to this entity!");

            component.OwnedSystem = system;
            component.Entity = entity;
            entity._components.Add(component);
        }

        public void UnregisterComponentForEntity(ComponentSystem system, Component component, Entity entity)
        {
            Debug.AssertDebug(system != null, "Component system cannot be null!");
            ValidEntityCheck(entity, "Invalid entity to remove component for!");
            Debug.AssertDebug(component.Entity != null, "Component is already removed from entity!");
            Debug.AssertDebug(entity._components.Contains(component), "Component was not found in entity!");

            component.Entity = null;
            entity._components.Remove(component);
        }
    }
}
