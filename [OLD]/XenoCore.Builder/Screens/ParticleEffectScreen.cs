using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using XenoCore.Engine.Screens;
using XenoCore.Engine.World;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Particles;
using XenoCore.Engine.GUI;
using XenoCore.Engine.Resources;
using XenoCore.Engine.Graphics;

namespace XenoCore.Builder.Screens
{
    public class ParticleEffectScreen : Screen
    {
        public ParticleSystem ParticleSystem { get; private set; }
        public uint Entity { get; private set; }
        private ParticleEffectDescription effect;

        private int maxParticles = 0;

        public bool ShowStats { get; set; } = true;

        public Camera Camera
        {
            get { return Systems.Get<CameraSystem>().Cameras[0]; }
        }

        public ParticleEffectScreen(ParticleEffectDescription effect = null)
        {
            Systems.Register(new EntitySystem());
            Systems.Register(new CameraSystem());
            Systems.Register(new WorldSystem(Systems, new Point(4096 * 1024, 4096 * 1024)));
            Systems.Register(ParticleSystem = new ParticleSystem(Systems));
            Systems.Register(new GUISystem());

            var container = new RelativeContainer();
            container.Add(new CameraDisplay()
            {
                Camera = Camera
            }, Vector2.Zero, Vector2.One);

            Systems.Get<GUISystem>().RootControl = container;

            Entity = Systems.Get<EntitySystem>().NewEntity();
            Systems.Get<WorldSystem>().AddComponent(Entity).Render = false;


            if (effect != null)
                SetParticleEffect(effect);
        }

        public void SetParticleEffect(ParticleEffectDescription effect)
        {
            this.effect = effect;

            ParticleSystem.Effects.Clear();
            ParticleSystem.AddParticleEffect(effect);

            if (ParticleSystem.GetComponent(Entity) != null)
                ParticleSystem.RemoveComponent(Entity);

           ParticleSystem.AddComponent(Entity, effect.Name);
        }

        public void Restart()
        {
            maxParticles = 0;

            ParticleSystem.RemoveComponent(Entity);

            SetParticleEffect(effect);
        }

        public override void Draw(GameTime gameTime)
        {
     

            base.Draw(gameTime);

            if (ShowStats)
            {
                int particles = ParticleSystem.GetComponent(Entity).Effect.Emitters.Sum(p => p.Particles.Count);
                maxParticles = Math.Max(particles, maxParticles);
                var instance = GraphicsService.Renderer.NewText(GraphicsService.Cache.GetFont("default"), 3, BlendingMode.Alpha);
                instance.Color = Color.White;
                instance.Text = $"Particles: {particles}/{maxParticles}";
                instance.Rotation = 0;
                instance.Destination.X = 4;
                instance.Destination.Y = 4;
            }

        }
    }
}
