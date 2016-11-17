using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Graphics;
using XenoCore.Engine.Systems.Entities;
using XenoCore.Engine.Systems.World;

namespace XenoCore.Engine.Systems.Rendering
{
    public class RenderingSystem : ComponentSystem, IDrawableSystem
    {
        private readonly GraphicsService GraphicsService;

        private ComponentContainer<RenderingComponent> textureComponents;

        public int DrawOrder
        {
            get
            {
                return 0;
            }
        }

        public RenderingSystem(SystemProvider systems)
        : this(systems.Get<EntitySystem>())
        {

        }

        public RenderingSystem(EntitySystem es)
            : base(es)
        {
            GraphicsService = ServiceProvider.Get<GraphicsService>();
            textureComponents = new ComponentContainer<RenderingComponent>(es.MaxEntites);
        }

        public RenderingComponent AddComponent(Entity entity)
        {
            var component = textureComponents.New(entity);
            component.Reset();
            EntitySystem.RegisterComponentForEntity(this, component, entity);

            return component;
        }
        public void RemoveComponent(Entity entity)
        {
            var component = textureComponents.Remove(entity);
            EntitySystem.UnregisterComponentForEntity(this, component, entity);
        }
        public RenderingComponent GetComponent(Entity entity)
        {
            return textureComponents.TryGet(entity);
        }
        public override void OnEntityDestroyed(Component systemComponent)
        {
            if (systemComponent is RenderingComponent)
                textureComponents.Remove(systemComponent as RenderingComponent);
        }

        public void Draw(DrawState state)
        {
            int count = textureComponents.Count;
            for (int i = 0; i < count; ++i)
            {
                RenderingComponent component = textureComponents[i];

                if (!component.IsVisible || component.IsCulled)
                    continue;

                Entity entity = component.Entity;
                RenderInstance instance = null;

                if (component.IsFont)
                {
                    SpriteFont font = GraphicsService.ResourceCache[component.Font];
                    Vector2 size = font.MeasureString(component.Text);

                    //TODO : State
                    instance = GraphicsService.Renderer.NewFont(component.Font, component.Layer, component.Blending, 1);
                    instance.TextScale = component.Size / size;
                    instance.Text = component.Text;
                    instance.Center = size / 2.0f;
                }
                else
                {
                    if (!component.TexturePart.HasValue)
                    {
                        Texture2D texture = GraphicsService.ResourceCache[component.Texture];
                        component.TexturePart = new Rectangle(0, 0, texture.Width, texture.Height);
                    }

                    Rectangle texturePart = component.TexturePart.Value;

                    instance = GraphicsService.Renderer.NewTexture(component.Texture, component.Layer, component.Blending,1);

                    instance.Destination.Width = (int)component.Size.X;
                    instance.Destination.Height = (int)component.Size.Y;

                    instance.TexturePart = texturePart;
                    instance.Center = new Vector2(texturePart.Width / 2, texturePart.Height / 2);
                }

                instance.Destination.X = (int)component.Position.X;
                instance.Destination.Y = (int)component.Position.Y;
                instance.Color = component.Color;
                instance.Effects = component.FlipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                instance.Effects |= component.FlipY ? SpriteEffects.FlipVertically : SpriteEffects.None;
                instance.Rotation = component.Rotation;
            }
        }
    }
}
