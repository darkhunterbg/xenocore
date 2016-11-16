using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Entities;

namespace XenoCore.Engine.Systems.World
{
    public class WorldSystem : ComponentSystem
    {
        private ComponentContainer<WorldComponent> components;

        public WorldSystem(SystemProvider systems)
           : this(systems.Get<EntitySystem>())
        {

        }
        public WorldSystem(EntitySystem es) :base(es)
        {
            components = new ComponentContainer<WorldComponent>(es.MaxEntites);
        }
        public WorldComponent AddComponent(Entity entity)
        {
            var component = components.New(entity);
            component.Reset();
            EntitySystem.RegisterComponentForEntity(this, component, entity);

            return component;
        }
        public void RemoveComponent(Entity entity)
        {
            var component = components.Remove(entity);
            EntitySystem.UnregisterComponentForEntity(this, component, entity);
        }
        public WorldComponent GetComponent(Entity entity)
        {
            return components.TryGet(entity);
        }
        public override void OnEntityDestroyed(Component systemComponent)
        {
            components.Remove(systemComponent as WorldComponent);
        }
    }
}
