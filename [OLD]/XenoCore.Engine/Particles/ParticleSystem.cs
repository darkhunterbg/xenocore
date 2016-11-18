using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Threading;
using XenoCore.Engine.Utilities;
using XenoCore.Engine.World;

namespace XenoCore.Engine.Particles
{
    public class ParticleSystem : ComponentSystem, IDrawableSystem
    {
        private ComponentContainer<ParticleComponent> components = new ComponentContainer<ParticleComponent>(EntitySystem.MAX_ENTITIES / 4);

        private WorldSystem worldSystem;

        private float deltaT;

        private PerThreadData<ParticleThreadData> threadData;

        private bool onlyRender;

        public Dictionary<String, ParticleEffectDescription> Effects { get; private set; } = new Dictionary<String, ParticleEffectDescription>();
        private Camera camera;
        private CameraSystem cameraSystem;

        public const int DRAW_ORDER = DrawingOrder.WORLD + 100;
        public int DrawOrder { get { return DRAW_ORDER; } }

        public ParticleSystem(EntitySystem es, WorldSystem worldSystem, CameraSystem cameraSystem)
            : base(es)
        {
            this.cameraSystem = cameraSystem;
            this.worldSystem = worldSystem;
            threadData = new PerThreadData<ParticleThreadData>();
            foreach (var data in threadData.Data)
            {
                data.SpawnContext.system = this;
                data.ResetContext.Random = data.Random;
            }
        }
        public ParticleSystem(SystemProvider systems)
           : this(systems.Get<EntitySystem>(), systems.Get<WorldSystem>(), systems.Get<CameraSystem>())
        {

        }
        public bool AddParticleEffect(ParticleEffectDescription effectDescription)
        {
            if (Effects.ContainsKey(effectDescription.Name))
                return false;
            Effects.Add(effectDescription.Name, effectDescription);

            foreach (var emitter in effectDescription.Emitters)
            {
                emitter.Required.Texture = GraphicsService.Cache.GetTexture(emitter.Required.TextureName);
                var raw = GraphicsService.Cache[emitter.Required.Texture];
                emitter.Required.TexturePart = new Rectangle(0, 0, raw.Width, raw.Height);

                GenerateTemplate(emitter);
            }
            return true;
        }
        public bool RenameParticleEffect(ParticleEffectDescription effectDescription, String newName)
        {
            if (Effects.ContainsKey(newName))
                return false;

            Effects.Remove(effectDescription.Name);
            effectDescription.Name = newName;
            AddParticleEffect(effectDescription);

            return true;

        }

        public ParticleComponent AddComponent(uint entityID, String effectName)
        {
            var component = components.New(entityID);
            component.Reset();

            EntitySystem.RegisterComponentForEntity(this, component, entityID);

            Debug.Assert(!String.IsNullOrEmpty(effectName), "Particle effect name cannot be null!");

            var effect = Effects[effectName];
            component.Effect = new ParticleEffect(effect);

            return component;
        }
        public void RemoveComponent(uint entityID)
        {
            var component = components.Remove(entityID);
            EntitySystem.UnregisterComponentForEntity(this, component, entityID);
        }
        public override void OnEntityDestroyed(Component systemComponent)
        {
            components.Remove(systemComponent as ParticleComponent);
        }
        public ParticleComponent GetComponent(uint entityID)
        {
            return components.TryGetByEntityId(entityID);
        }

        public void Draw(DrawState state)
        {
            //TODO : own random class with reseeding after some iterations count
            deltaT = state.DeltaT;

            this.onlyRender = state.Paused;

            for (int i = 0; i < components.Count; ++i)
            {
                components[i]._updated = false;
            }

            foreach (var camera in cameraSystem.Cameras)
            {
                if (!camera.Used)
                    continue;

                this.camera = camera;

                var task = JobServiceExtender.RunJobs(components, UpdateEffectJob);
                JobService.WaitForJobs(task);
            }
        }

        private void UpdateEffectJob(Object param)
        {
            ParticleComponent component = param as ParticleComponent;

            var worldComponent = worldSystem.GetComponent(component.Entity.ID);
            var effect = component.Effect;

            var threadData = this.threadData.Current;

            bool onlyRender = this.onlyRender || component._updated;

            if (camera.ViewRect.Contains(worldComponent.Position.X, worldComponent.Position.Y))
            {
                for (int i = 0; i < effect.Emitters.Length; ++i)
                {
                    var emitter = effect.Emitters[i];

                    if (!onlyRender)
                    {
                        ParticlesElapseLife(emitter, threadData, deltaT);
                        UpdateEmitter(emitter, worldComponent.Position, threadData, deltaT);
                    }

                    UpdateParticles(emitter, threadData, onlyRender, deltaT);
                }

                component._updated = true;
            }
        }
        private void UpdateEmitter(ParticleEmitter emitter, Vector2 worldPosition, ParticleThreadData threadData, float time)
        {
            int emitCount = 0;
            bool hasDelay = emitter.Description.Required.Delay > 0;
            var context = threadData.SpawnContext;

            context.Emitter = emitter;
            context.emitterPosition = worldPosition;


            while (time > 0)
            {
                if (hasDelay && emitter._delay < emitter.Description.Required.Delay)
                {
                    emitter._delay += time;
                    time = 0;
                    if (emitter._delay > emitter.Description.Required.Delay)
                        time = emitter._delay - emitter.Description.Required.Delay;
                    continue;
                }
                else
                {
                    if (emitter._duration >= emitter.Description.Required.Duration)
                        ResetEmitter(emitter, threadData);

                    emitter._duration += time;
                    time = 0;

                    if (emitter._duration >= emitter.Description.Required.Duration)
                    {
                        time = emitter._duration - emitter.Description.Required.Duration;
                        emitter._duration = emitter.Description.Required.Duration;
                    }

                    context.EmitterTime = emitter._duration;


                    for (int i = 0; i < emitter.Description.Modules.Count; ++i)
                    {
                        var module = emitter.Description.Modules[i];

                        if (!module.Flags.HasFlag(ParticleModuleFlag.Spawner))
                            continue;

                        context.Data = emitter.ModuleData[i];
                        int emit = module.GetNewParticlesCount(context);
                        Debug.Assert(emit >= 0, "Emit was negative!");

                        if (emit > 0)
                            module.SpawnParticles(context, emit);

                        emitCount += emit;


                    }


                }
            }
        }

        private void ResetEmitter(ParticleEmitter emitter, ParticleThreadData threadData)
        {
            emitter.spawned = 0;
            emitter._duration = 0;
            emitter._delay = 0;

            var context = threadData.ResetContext;

            emitter.Description.StartTemplate.Reseed(threadData.Random);

            for (int i = 0; i < emitter.Description.Modules.Count; ++i)
            {
                var module = emitter.Description.Modules[i];

                if (!module.Flags.HasFlag(ParticleModuleFlag.Reset))
                    continue;

                context.Data = emitter.ModuleData[i];

                module.Reset(context);
            }
        }

        internal Particle EmitParticle(ParticleEmitter emitter, ref Vector2 worldPos)
        {
            Particle particle = null;
            var data = threadData.Current;

            if (emitter.Particles.Count == emitter.Particles.Size)
                particle = emitter.Particles[0];
            else
                particle = emitter.Particles.New();

            byte seed = data.Random.NextByte();

            particle.Position = worldPos;
            particle.LoadTemplate(emitter.Description.StartTemplate, seed);
            particle.New = true;

            foreach (var module in emitter.Description.Modules)
            {
                if (!module.Flags.HasFlag(ParticleModuleFlag.Initializer))
                    continue;

                module.InitializeParticle(particle, emitter.spawned, emitter.Description);
            }
            emitter.spawned++;


            return particle;
        }

        private void ParticlesElapseLife(ParticleEmitter emitter, ParticleThreadData threadData, float time)
        {
            for (int i = 0; i < emitter.Particles.Count; ++i)
            {
                var particle = emitter.Particles[i];
                particle.ElaspedLife += time;

                if (particle.ElaspedLife > particle.StartLife)
                {
                    emitter.Particles.RemoveAt(i);
                    --i;
                    continue;
                }
            }
        }
        private void UpdateParticles(ParticleEmitter emitter, ParticleThreadData threadData, bool onlyRender, float time)
        {
            threadData.OperatorContext.Template = emitter.Description.StartTemplate;

            var state = threadData.OperatorContext.State;

            for (int i = 0; i < emitter.Particles.Count; ++i)
            {
                var particle = emitter.Particles[i];

                float lerp = particle.ElaspedLife / particle.StartLife;

                state.Load(emitter.Description.StartTemplate, emitter.Description.EndTemplate, particle.Seed, lerp);

                for (int j = 0; j < emitter.Description.Modules.Count; ++j)
                {
                    var module = emitter.Description.Modules[j];
                    if (!module.Flags.HasFlag(ParticleModuleFlag.Updater))
                        continue;

                    threadData.OperatorContext.Data = emitter.ModuleData[j];

                    module.ApplyOperator(particle, threadData.OperatorContext, lerp);
                }

                if (!onlyRender && !particle.New)
                {
                    float drag = MathHelper.Lerp(emitter.Description.StartTemplate.Drag.Get(particle.Seed),
                        emitter.Description.EndTemplate.Drag.Get(particle.Seed), lerp);

                    particle.Velocity += (particle.Acceleration * time);
                    particle.Velocity *= 1.0f - drag * 6 * time;
                    particle.Position += particle.Velocity * time;

                }

                particle.New = false;

                state.Rotation += state.RotationRate * particle.ElaspedLife;
                state.Position = particle.Position;

                RenderParticle(emitter, state);
            }


        }
        private void RenderParticle(ParticleEmitter emitter, ParticleState state)
        {
            RectangleF rect = new RectangleF();
            var required = emitter.Description.Required;

            rect.Size = state.Size * new Vector2(required.TexturePart.Width, required.TexturePart.Height);
            rect.Center = state.Position;

            var instance = GraphicsRendererExtender.NexTextureRelativeToCamera(camera, ref rect,
                required.Texture, required.Blending, 0, false);


            instance.TexturePart = required.TexturePart;
            instance.Rotation = state.Rotation;
            instance.Center = new Vector2(instance.TexturePart.Width / 2, instance.TexturePart.Height / 2);
            instance.Text = null;
            instance.Color = state.Color * state.Alpha;
        }

        private void GenerateTemplate(ParticleEmitterDescription emitter)
        {
            emitter.TotalEmittingParticles = 0;

            var spawnContext = threadData.Current.SpawnContext;
            spawnContext.EmitterTime = emitter.Required.Duration;
            spawnContext.Emitter = new ParticleEmitter(emitter);

            var resetContext = threadData.Current.ResetContext;

            foreach (var module in emitter.Modules.Where(p => p.Flags.HasFlag(ParticleModuleFlag.Spawner)))
            {
                spawnContext.Data = module.NewModuleData();
                emitter.TotalEmittingParticles += module.GetNewParticlesCount(spawnContext);
            }


            var startTemplate = emitter.StartTemplate = new ParticleTemplate();
            var endTempalte = emitter.EndTemplate = new ParticleTemplate();
            var context = new ParticleTemplateContext()
            {
                Emitter = emitter,
            };

            for (int i = 0; i < emitter.Modules.Count; ++i)
            {
                var module = emitter.Modules[i];

                if (module.Flags.HasFlag(ParticleModuleFlag.StartTemplate))
                {
                    module.ApplyToTemplate(startTemplate, context);
                    module.ApplyToTemplate(endTempalte, context);
                }
                if (module.Flags.HasFlag(ParticleModuleFlag.EndTemplate))
                    module.ApplyToTemplate(endTempalte, context);
            }


        }
    }
}
