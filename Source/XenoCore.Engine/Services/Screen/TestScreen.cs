using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Graphics;
using XenoCore.Engine.Systems.Entities;
using XenoCore.Engine.Systems.Rendering;

namespace XenoCore.Engine.Services.Screen
{
    public class TestScreen : Screen
    {
        public EntitySystem EntitySystem { get; private set; }
        public RenderingSystem RenderingSystem { get; private set; }
        private Entity entity;
        public TestScreen()
        {
            int size = 4096;
            Systems.Add(EntitySystem = new EntitySystem(size));
            Systems.Add(RenderingSystem = new RenderingSystem(Systems));

            entity = EntitySystem.NewEntity();
            RenderingComponent component = RenderingSystem.AddComponent(entity);
              component.Texture = ServiceProvider.Get<GraphicsService>().ResourceCache.Textures.Get("earth");
            component.Text = "WTF";
            component.Font = ServiceProvider.Get<GraphicsService>().ResourceCache.Fonts.Get("default");
            component.Position = new Vector2(100, 100);
            component.Scale *= 2;
        }

      
    }
}
