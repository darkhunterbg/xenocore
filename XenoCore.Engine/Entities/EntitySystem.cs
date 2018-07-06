using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XenoCore.Engine.Entities
{
    public class EntitySystem
    {
        private uint lastid = 0;
        private Queue<uint> recycledEntities = new Queue<uint>();

        public const int MAX_ENTITIES = 4096;

        private Entity[] entities = new Entity[MAX_ENTITIES];


        public EntitySystem()
        {
            for (int i = 0; i < entities.Length; ++i)
            {
                entities[i] = new Entity()
                {
                    ID = (uint)i,
                };
            }
        }

        public uint NewEntity()
        {

            uint id = 0;

            if (recycledEntities.Count == 0)
            {
                id = ++lastid;
            }
            else
            {
                id = recycledEntities.Dequeue();
            }

            entities[id].ParentID = 0;
            entities[id].Children.Clear();
            entities[id].ChildDepth = 0;
            entities[id].Used = true;

            return id;
        }
        public void DeleteEntity(uint id)
        {
            Debug.Assert(id <= lastid && !recycledEntities.Contains(id), "Invalid entityID to delete!");

            foreach (uint child in entities[id].Children)
                DeleteEntity(child);


            foreach (var component in entities[id].Components)
                component.OwnedSystem.OnEntityDestroyed(component);

            entities[id].Components.Clear();
            entities[id].Used = false;
            recycledEntities.Enqueue(id);
        }

        public void AddChild(uint id, uint childid)
        {
            Debug.Assert(id <= lastid && !recycledEntities.Contains(id), "Invalid entityID to add children to!");
            Debug.Assert(childid <= lastid && !recycledEntities.Contains(childid), "Invalid entityID to become a child!");

            if (!entities[id].Children.Contains(childid))
                entities[id].Children.Add(childid);

            entities[childid].ParentID = id;
            ++entities[childid].ChildDepth;
        }
        public void RemoveChild(uint id, uint childid)
        {
            Debug.Assert(id <= lastid && !recycledEntities.Contains(id), "Invalid entityID to remove child from!");
            Debug.Assert(childid <= lastid && !recycledEntities.Contains(childid), "Invalid entityID for remove child!");
            Debug.Assert(entities[childid].ChildDepth > 0, "Invalid entityID for child!");

            entities[id].Children.Remove(childid);
            --entities[childid].ChildDepth;
        }

        public Entity GetEntityInfo(uint id)
        {
            Debug.Assert(id <= lastid && !recycledEntities.Contains(id), "Invalid entityID to get info!");
            return entities[id];
        }

        public void RegisterComponentForEntity(ComponentSystem system, Component component, uint id)
        {
            Debug.Assert(system != null, "Component system cannot be null!");
            Debug.Assert(id <= lastid && !recycledEntities.Contains(id), "Invalid entityID to add component for!");

            component.OwnedSystem = system;
            component.Entity = entities[id];
            entities[id].Components.Add(component);
        }

        public void UnregisterComponentForEntity(ComponentSystem system, Component component, uint id)
        {
            Debug.Assert(system != null, "Component system cannot be null!");
            Debug.Assert(id <= lastid && !recycledEntities.Contains(id), "Invalid entityID to remove component for!");

            component.Entity = null;
            entities[id].Components.Remove(component);
        }

        public List<Entity> GetAllEntities()
        {
            return entities.Where(p => p.Used).ToList();
        }
    }
}
