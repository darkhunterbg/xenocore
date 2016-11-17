using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Entities;
using XenoCore.Engine.Systems.Rendering;
using XenoCore.Engine.Systems.World;

namespace XenoCore.Engine.Systems.Animation
{
    public class AnimationSystem : ComponentSystem, IUpdateableSystem
    {
        private ComponentContainer<AnimationComponent> animationComponents;

        private readonly RenderingSystem RenderingSystem;
        private readonly WorldSystem WorldSystem;

        public AnimationSystem(SystemProvider systems)
            : this(systems.Get<EntitySystem>(), systems.Get<RenderingSystem>(), systems.Get<WorldSystem>()) { }
        public AnimationSystem(EntitySystem es, RenderingSystem renderingSystem, WorldSystem ws) : base(es)
        {
            animationComponents = new ComponentContainer<AnimationComponent>(es.MaxEntites);
            RenderingSystem = renderingSystem;
            WorldSystem = ws;
        }

        public int UpdateOrder
        {
            get
            {
                return 0;
            }
        }


        public AnimationComponent AddComponent(Entity entity)
        {
            var component = animationComponents.New(entity);
            component.Reset();
            EntitySystem.RegisterComponentForEntity(this, component, entity);

            return component;
        }
        public void RemoveComponent(Entity entity)
        {
            var component = animationComponents.Remove(entity);
            EntitySystem.UnregisterComponentForEntity(this, component, entity);
        }
        public AnimationComponent GetComponent(Entity entity)
        {
            return animationComponents.TryGet(entity);
        }
        public override void OnEntityDestroyed(Component systemComponent)
        {
            animationComponents.Remove(systemComponent as AnimationComponent);
        }

        public void Update(UpdateState state)
        {
            int count = animationComponents.Count;
            for (int i = 0; i < count; ++i)
            {
                AnimationComponent component = animationComponents[i];
                Entity entity = component.Entity;

             


                SpriteAnimationFrame frame = component.Animation.Frames[component.FrameIndex];
                bool changeFrame = component.FrameElapsedTime > frame.Duration;

                if (!component.Paused)
                {
                    component.FrameElapsedTime += state.DeltaT * component.AnimationSpeed;
                    while (changeFrame)
                    {
                        if (component.FrameIndex < component.Animation.Frames.Count - 1)
                        {
                            component.FrameElapsedTime -= frame.Duration;
                            ++component.FrameIndex;
                        }
                        else
                        {
                            if (component.Animation.Loop)
                            {
                                component.FrameElapsedTime -= frame.Duration;
                                component.FrameIndex = 0;
                            }
                            else
                            {
                                component.FrameElapsedTime = frame.Duration;
                                break;
                            }
                        }

                        frame = component.Animation.Frames[component.FrameIndex];
                        changeFrame = component.FrameElapsedTime > frame.Duration;
                    }
                }

                RenderingComponent renderingComponent = RenderingSystem.GetComponent(entity);
                if (renderingComponent != null)
                {
                    renderingComponent.Texture = component.Animation.SpriteSheet.Texture;
                    renderingComponent.TexturePart = frame.Sprite.Region;
                    renderingComponent.Size = new Vector2(frame.Sprite.Region.Width, frame.Sprite.Region.Height);
                }
                WorldComponent worldComponent = WorldSystem.GetComponent(entity);
                if (worldComponent != null)
                {
                    worldComponent.BaseSize = new Vector2(frame.Sprite.Region.Width, frame.Sprite.Region.Height);
                }

            }


        }
    }
}
