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
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/Modules/Rotation/index.html
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/Modules/RotationRate/index.html#initialrotrate

    public class InitialRotationModule : ParticleModule
    {
        [PropertiesEditor]
        [DistributionEditor]
        public RandomFloat InitialRotation { get; set; }= new RandomFloat(0);

        public InitialRotationModule()
         : base( ParticleModuleFlag.StartTemplate)
        {

        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Rotation.Max += InitialRotation.Max * MathHelper.TwoPi;

            if(InitialRotation.UseRandom)
            {
                template.Rotation.Min += InitialRotation.Min * MathHelper.TwoPi;
                template.Rotation.UseRandom = true;
            }
        }
    }

    public class RotationOverLifeModule : ParticleModule
    {
        [PropertiesEditor]
        [DistributionEditor]
        public RandomFloat RotatioOverLife { get; set; } = new RandomFloat(0);

        [BooleanEditor]
        public bool Scale { get; set; } = false;

        public RotationOverLifeModule()
         : base(ParticleModuleFlag.EndTemplate)
        {

        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            if (Scale)
                template.Rotation.Max *= RotatioOverLife.Max;
            else
                template.Rotation.Max += RotatioOverLife.Max * MathHelper.TwoPi;

            if (RotatioOverLife.UseRandom)
            {

                if (Scale)
                    template.Rotation.Min *= RotatioOverLife.Min;
                else
                    template.Rotation.Min += RotatioOverLife.Min * MathHelper.TwoPi;

                template.Rotation.UseRandom = true;
            }
        }
    }

    public class InitialRotatioRateModule : ParticleModule
    {
        [PropertiesEditor]
        [FloatEditor]
        public RandomFloat StartRotationRate { get; set; } = new RandomFloat(1);

        public InitialRotatioRateModule()
         : base( ParticleModuleFlag.StartTemplate)
        {

        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.RotationRate.Max += StartRotationRate.Max * MathHelper.TwoPi;

            if (StartRotationRate.UseRandom)
            {
                template.RotationRate.Min += StartRotationRate.Min * MathHelper.TwoPi;
                template.RotationRate.UseRandom = true;
            }
        }
    }

    public class RotationRateLifeScaleModule : ParticleModule
    {
        [PropertiesEditor]
        [FloatEditor]
        public RandomFloat LifeMultiplier { get; set; } = new RandomFloat(1);


        public RotationRateLifeScaleModule()
         : base( ParticleModuleFlag.EndTemplate)
        {

        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.RotationRate.Max += LifeMultiplier.Max;

            if (LifeMultiplier.UseRandom)
            {
                template.RotationRate.Min += LifeMultiplier.Min * MathHelper.TwoPi;
                template.RotationRate.UseRandom = true;
            }
        }
    }
}
