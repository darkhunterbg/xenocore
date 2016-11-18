using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Systems.Entities
{
    public abstract class ComponentSystem
    {
        public EntitySystem EntitySystem { get; internal set; }

        public ComponentSystem(EntitySystem es)
        {
            EntitySystem = es;
        }

        public abstract void OnEntityDestroyed(Component systemComponent);
    }
}
