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
    //   https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/Modules/Size/index.html

    public class InitialSizeModule : ParticleModule
    {
        [PropertiesEditor]
        [VectorEditor(VectorValueType.Any, Min = 0, Step = 0.1)]
        public RandomVector2 InitialSize { get; set; } = new RandomVector2(Vector2.One);

        public InitialSizeModule() :
            base(ParticleModuleFlag.StartTemplate)
        {
        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Size.Max *= InitialSize.Max;
            if (InitialSize.UseRandom)
            {
                template.Size.Min *= InitialSize.Min;
                template.Size.UseRandom = true;
            }
        }
    }

    public class SizeByLifeModule : ParticleModule
    {
        [PropertiesEditor]
        [FloatEditor(Min = 0)]
        public RandomFloat LifeMultiplier { get; set; } = new RandomFloat( 2.0f );

        [BooleanEditor]
        public bool MultiplyX { get; set; } = true;

        [BooleanEditor]
        public bool MultiplyY { get; set; } = true;

        public SizeByLifeModule() :
            base(ParticleModuleFlag.EndTemplate)
        {
        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            Vector2 size = new Vector2(MultiplyX ? LifeMultiplier.Max : 1.0f, MultiplyY ? LifeMultiplier.Max : 1.0f);

            template.Size.Max *= size;
            if (LifeMultiplier.UseRandom)
            {
                size = new Vector2(MultiplyX ? LifeMultiplier.Min : 1.0f, MultiplyY ? LifeMultiplier.Min : 1.0f);
                template.Size.Min *= size;
                template.Size.UseRandom = true;
            }
        }
    }

    public class SizeBySpeedModule : ParticleModule
    {
        [FloatEditor(Min = 0, Step = 0.01, Format = "F3")]
        public float SpeedScale { get; set; } = 1.0f;

        [FloatEditor(Min = 0)]
        public float MaxScale { get; set; } = 2.0f;

        public SizeBySpeedModule() :
            base(ParticleModuleFlag.Updater)
        {
        }

        public override void ApplyOperator(Particle particle, ParticleOperatorContext context, float lerp)
        {
            float scale = Math.Min(particle.Velocity.Length() * SpeedScale, MaxScale);
            context.State.Size *= scale;
        }
    }


}
