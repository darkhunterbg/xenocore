using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Particles
{

    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/Modules/Lifetime/index.html
    public class LifetimeModule : ParticleModule
    {
        public class LifetimeRandom
        {
            [DistributionEditor]
            public float Min { get; set; }

            [DistributionEditor]
            public float Max { get; set; } = 1.0f;

            [BooleanEditor]
            public bool UseRandom { get; set; }

            public LifetimeRandom() { }
            public LifetimeRandom(float max)
            {
                Max = max;
            }
            public LifetimeRandom(float min, float max)
            {
                Min = min;
                Max = max;
                UseRandom = true;
            }
        }

        [PropertiesEditor]
        [DistributionEditor]
        public RandomFloat Lifetime { get; set; } = new RandomFloat(1.0f);

        public LifetimeModule() :
            base(ParticleModuleFlag.StartTemplate)
        {
        }


        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Life.Max += Lifetime.Max * context.Emitter.Required.Duration;

            if (Lifetime.UseRandom)
            {
                template.Life.Min += Lifetime.Min * context.Emitter.Required.Duration;
                template.Life.UseRandom = true;
            }
        }
    }
}
