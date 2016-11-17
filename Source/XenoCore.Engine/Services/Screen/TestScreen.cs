using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Graphics;
using XenoCore.Engine.Services.Input;
using XenoCore.Engine.Systems.Animation;
using XenoCore.Engine.Systems.Entities;
using XenoCore.Engine.Systems.Events;
using XenoCore.Engine.Systems.Rendering;
using XenoCore.Engine.Systems.Scripting;
using XenoCore.Engine.Systems.World;

namespace XenoCore.Engine.Services.Screen
{
    public class TestScreen : Screen
    {
        public EntitySystem EntitySystem { get; private set; }
        public RenderingSystem RenderingSystem { get; private set; }
        public WorldSystem WorldSystem { get; private set; }
        public CameraSystem CameraSystem { get; private set; }
        public AnimationSystem AnimationSystem { get; private set; }
        public ScriptingSystem ScriptingSystem { get; private set; }
        public EventSystem EventSystem { get; private set; }

        private Dictionary<String, SpriteAnimation> animations = new Dictionary<string, SpriteAnimation>();

        private Entity entity;
        public TestScreen()
        {
            int size = 4096;
            Systems.Add(EntitySystem = new EntitySystem(size));
            Systems.Add(CameraSystem = new CameraSystem());
            Systems.Add(RenderingSystem = new RenderingSystem(Systems));
            Systems.Add(WorldSystem = new WorldSystem(Systems));
            Systems.Add(AnimationSystem = new AnimationSystem(Systems));
            Systems.Add(ScriptingSystem = new ScriptingSystem(Systems));
            Systems.Add(EventSystem = new EventSystem(Systems));

            entity = EntitySystem.NewEntity();
            RenderingComponent component = RenderingSystem.AddComponent(entity);
            Texture t = ServiceProvider.Get<GraphicsService>().ResourceCache.Textures.Get("spritesheet");
            SpriteSheet ss = new SpriteSheet(t);

            ss.Sprites.Add(new Sprite(new Rectangle(2, 2, 33, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(34, 2, 40, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(74, 2, 40, 45)));

            ss.Sprites.Add(new Sprite(new Rectangle(0, 90, 33, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(33, 90, 33, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(68, 90, 46, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(125, 90, 37, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(162, 90, 31, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(193, 90, 29, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(222, 90, 31, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(253, 90, 41, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(300, 90, 35, 45)));
            ss.Sprites.Add(new Sprite(new Rectangle(336, 90, 30, 45)));

            WorldComponent w = WorldSystem.AddComponent(entity);
            w.Position = new Vector2(0, 0);
            w.Scale *= 2;


            SpriteAnimation animation = new SpriteAnimation("walk", ss) { Loop = true };
            for (int i = 3; i < 13; ++i)
                animation.Frames.Add(new SpriteAnimationFrame(ss.Sprites[i], 0.08f));
            animations.Add(animation.Name, animation);

            animation = new SpriteAnimation("stand", ss) { Loop = true };
            animation.Frames.Add(new SpriteAnimationFrame(ss.Sprites[0], 1f));
            animation.Frames.Add(new SpriteAnimationFrame(ss.Sprites[1], 0.15f));
            animation.Frames.Add(new SpriteAnimationFrame(ss.Sprites[2], 0.15f));
            animations.Add(animation.Name, animation);


            AnimationComponent a = AnimationSystem.AddComponent(entity);
            a.Animation = animations["stand"];
        }


        public override void UpdateInput(GameTime gameTime)
        {
            base.UpdateInput(gameTime);

            AnimationComponent a = entity.GetComponent<AnimationComponent>();

            InputService input = ServiceProvider.Get<InputService>();
            if (input.State.WasKeyPressed(Keys.A))
            {
                a.SetAnimation(animations["walk"], true);
                entity.GetComponent<RenderingComponent>().FlipX = true;
            }
            if (input.State.WasKeyPressed(Keys.D))
            {
                a.SetAnimation(animations["walk"], true);
                entity.GetComponent<RenderingComponent>().FlipX = false;
            }

            if (input.State.CurrentInput.Keyboard.IsKeyUp(Keys.A) &&
                input.State.CurrentInput.Keyboard.IsKeyUp(Keys.D))
            {
                a.SetAnimation(animations["stand"]);
            }
        }

        public override void Update(GameTime gameTime, bool paused)
        {
            base.Update(gameTime, paused);
        }
    }
}
