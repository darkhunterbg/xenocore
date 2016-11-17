using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Graphics;
using XenoCore.Engine.Systems.Entities;
using XenoCore.Engine.Systems.Rendering;
using XenoCore.Engine.Systems.World;

namespace XenoCore.Engine.Services.Screen
{
    public class TestScreen : Screen
    {
        public EntitySystem EntitySystem { get; private set; }
        public RenderingSystem RenderingSystem { get; private set; }
        public WorldSystem WorldSystem { get; private set; }
        public CameraSystem CameraSystem { get; private set; }

        private Entity entity;
        public TestScreen()
        {
            int size = 4096;
            Systems.Add(EntitySystem = new EntitySystem(size));
            Systems.Add(CameraSystem = new CameraSystem());
            Systems.Add(RenderingSystem = new RenderingSystem(Systems));
            Systems.Add(WorldSystem = new WorldSystem(Systems));

            entity = EntitySystem.NewEntity();
            RenderingComponent component = RenderingSystem.AddComponent(entity);
            component.Texture = ServiceProvider.Get<GraphicsService>().ResourceCache.Textures.Get("earth");
          //  component.Font = ServiceProvider.Get<GraphicsService>().ResourceCache.Fonts.Get("default");
         //   component.Text = "TEST";

            WorldComponent w = WorldSystem.AddComponent(entity);
            w.Position = new Vector2(0, 0);
            w.BaseSize = new Vector2(800, 600);

           // CameraSystem.CurrentCamera.Zoom = 1.0f;
        }


        public override void Update(GameTime gameTime, bool paused)
        {
            base.Update(gameTime, paused);
        }
    }
}
