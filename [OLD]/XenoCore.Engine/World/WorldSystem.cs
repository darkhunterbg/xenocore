using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Threading;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.World
{

    public class WorldSystem : ComponentSystem, IDrawableSystem
    {
        private ComponentContainer<WorldComponent> components = new ComponentContainer<WorldComponent>(EntitySystem.MAX_ENTITIES);

        private List<WorldComponent>[] groupedComponents = new List<WorldComponent>[8];

        const int JOB_BATCH_SIZE = 100;

        private int groupIndex = 0;

        public const int DRAW_ORDER = DrawingOrder.WORLD;
        public int DrawOrder { get { return DRAW_ORDER; } }

        public Point WorldSize { get; private set; }

        private CameraSystem cameraSystem;
        private Camera camera;

        public WorldSystem(EntitySystem es, CameraSystem cameraSystem, Point worldSize) : base(es)
        {

            this.WorldSize = worldSize;
            this.cameraSystem = cameraSystem;

            for (int i = 0; i < groupedComponents.Length; ++i)
                groupedComponents[i] = new List<WorldComponent>();
        }
        public WorldSystem(SystemProvider systems, Point worldSize)
           : this(systems.Get<EntitySystem>(), systems.Get<CameraSystem>(), worldSize)
        {

        }

        public WorldComponent AddComponent(uint entityID)
        {
            var component = components.New(entityID);
            component.Reset();
            EntitySystem.RegisterComponentForEntity(this, component, entityID);

            return component;
        }
        public void RemoveComponent(uint entityID)
        {
            var component = components.Remove(entityID);
            EntitySystem.UnregisterComponentForEntity(this, component, entityID);
        }
        public WorldComponent GetComponent(uint entityID)
        {
            return components.TryGetByEntityId(entityID);
        }
        public override void OnEntityDestroyed(Component systemComponent)
        {
            components.Remove(systemComponent as WorldComponent);
        }

        public void Draw(DrawState state)
        {

            //Strategy : split to groups by child depth;
            //Max 8 depth
            foreach (var list in groupedComponents)
                list.Clear();

            for (int i = 0; i < components.Count; ++i)
            {
                var component = components[i];
                groupedComponents[component.Entity.ChildDepth].Add(component);
            }

            foreach (var cam in cameraSystem.Cameras)
            {
                if (!cam.Used)
                    continue;

                this.camera = cam;

                for (int j = 0; j < groupedComponents.Length; ++j)
                {
                    var componentsList = groupedComponents[j];
                    if (componentsList.Count == 0)
                        continue;

                    groupIndex = j;

                    var task = JobServiceExtender.RunJobsBatched(componentsList.Count, JOB_BATCH_SIZE, UpdateComponentsJob);
                    JobService.WaitForJobs(task);
                }
            }
        }
        private void UpdateComponentsJob(Object param)
        {
            BatchedJobData data = (BatchedJobData)param;
            RectangleF rect = new RectangleF();

            for (int i = data.StartIndex; i < data.EndIndex; ++i)
            {
                var component = groupedComponents[groupIndex][i];
                ProcessComponent(component, ref rect);
            }
        }

        private void ProcessComponent(WorldComponent component, ref RectangleF rect)
        {
            if (!component.Render)
                return;

            if (component.Entity.ParentID > 0)
            {
                var parentComponent = GetComponent(component.Entity.ParentID);
                component.Position = parentComponent.Position + component.ParentOffset;
            }


            RenderInstance instance = null;
            //TODO : text will be a separate component

            //if (!String.IsNullOrEmpty(component.Text))
            //{
            //    instance = GraphicsService.Renderer.NewText(component.Font.Value, 0, component.Blending);
            //    instance.Destination.X = (int)(component.Position.X );
            //    instance.Destination.Y = (int)(component.Position.Y );
            //    instance.Text = component.Text;

            //}
            //else
            //{

            rect.Size = component.ActualSize;
            rect.Center = component.Position;

            instance = GraphicsRendererExtender.NexTextureRelativeToCamera(
                camera, ref rect, component.Texture, component.Blending, 0, true);

            if (instance == null)
                return;

            instance.TexturePart.Width = (int)component.BaseSize.X;
            instance.TexturePart.Height = (int)component.BaseSize.Y;
            instance.Rotation = component.Rotation;
            instance.Center = new Vector2(instance.TexturePart.Width / 2, instance.TexturePart.Height / 2);

            instance.Color = component.Color;
        }

    }
}
