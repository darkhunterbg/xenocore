using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Particles
{
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/Modules/Velocity/index.html#velocity/life

    public class InitialVelocityModule : ParticleModule
    {
        [PropertiesEditor]
        [FloatEditor]
        public RandomFloat StartVelocity { get; set; } = new RandomFloat(100);

        [PropertiesEditor]
        [AngleEditor]
        public RandomFloat StartDirectionRadial { get; set; } = new RandomFloat(0);

        public InitialVelocityModule() :
            base(ParticleModuleFlag.StartTemplate | ParticleModuleFlag.Initializer)
        {
        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Velocity.Max += StartVelocity.Max * -Vector2.UnitY;

            if (StartVelocity.UseRandom)
            {
                template.Velocity.Min += StartVelocity.Min * -Vector2.UnitY;
                template.Velocity.UseRandom = true;
            }
        }

        public override void InitializeParticle(Particle particle, int spawnIndex, ParticleEmitterDescription emitter)
        {
            if (StartDirectionRadial.UseRandom)
            {
                var rotate = StartDirectionRadial.Get(particle.Seed);
                particle.Velocity = particle.Velocity.Rotate(rotate * MathHelper.Pi);
            }
            else
                particle.Velocity = particle.Velocity.Rotate(StartDirectionRadial.Max * MathHelper.Pi);
        }
    }


    public class ConeVelocityModule : ParticleModule
    {
        [DistributionEditor]
        public float Angle { get; set; } = 0.25f;

        [PropertiesEditor]
        [FloatEditor]
        public RandomFloat Velocity { get; set; } = new RandomFloat(100);

        //  [PropertiesEditor]
        [AngleEditor]
        public float DirectionRadial { get; set; } = 0;

        public ConeVelocityModule() :
                base(ParticleModuleFlag.Initializer)
        {
        }

        public override void InitializeParticle(Particle particle, int spawnIndex, ParticleEmitterDescription emitter)
        {
            int count = emitter.TotalEmittingParticles;

            var cone = Angle * MathHelper.TwoPi;
            var rotate = MathHelper.Pi * DirectionRadial;// (DirectionRadial.UseRandom ? DirectionRadial.Get(particle.Seed): DirectionRadial.Max  );
            Vector2 velocity = new Vector2(0, -1).Rotate(rotate) * (Velocity.UseRandom ?  Velocity.Get(particle.Seed) : Velocity.Max);

            float step = 0;

            if (cone > MathHelper.Pi)
                step = cone / count;
            else
                step = cone / (count - 1);

            if (count > 1)
                cone = cone * -0.5f;
            else
                cone = 0;

            cone += step * spawnIndex;

            particle.Velocity += velocity.Rotate(cone);

        }

       

    }

}
