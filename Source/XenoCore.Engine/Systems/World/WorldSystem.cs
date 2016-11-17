using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Entities;
using XenoCore.Engine.Systems.Rendering;

namespace XenoCore.Engine.Systems.World
{
    public class WorldSystem : ComponentSystem, IDrawableSystem
    {
        private ComponentContainer<WorldComponent> components;

        private readonly RenderingSystem RenderingSystem;
        private readonly CameraSystem CameraSystem;

        public int DrawOrder
        {
            get
            {
                return -1;
            }
        }

        public WorldSystem(SystemProvider systems)
           : this(systems.Get<EntitySystem>(), systems.Get<CameraSystem>(), systems.Get<RenderingSystem>())
        {

        }
        public WorldSystem(EntitySystem es, CameraSystem cs, RenderingSystem rs) : base(es)
        {
            components = new ComponentContainer<WorldComponent>(es.MaxEntites);
            CameraSystem = cs;
            RenderingSystem = rs;
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

        public void Draw(DrawState state)
        {
            Camera camera = CameraSystem.CurrentCamera;

            RectangleF rect;

            int count = components.Count;
            for (int i = 0; i < components.Count; ++i)
            {
                WorldComponent component = components[i];
                Entity entity = component.Entity;

                RenderingComponent renderingComponent = RenderingSystem.GetComponent(entity);

                if (renderingComponent != null)
                {
                    rect = new RectangleF(component.Position - component.ActualSize / 2.0f, component.Position + component.ActualSize / 2.0f);

                    camera.ViewRect.Intersects(ref rect, out renderingComponent.IsCulled);
                    renderingComponent.IsCulled = !renderingComponent.IsCulled;

                    if (!renderingComponent.IsCulled)
                    {
                        renderingComponent.Position = component.Position - camera.ViewRect.Min;
                        renderingComponent.Rotation = component.Rotation;
                        renderingComponent.Size = component.ActualSize;
                    }
                }
            }
        }
    }
}
