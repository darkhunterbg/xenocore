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
    public class AccelerationModule : ParticleModule
    {
        [PropertiesEditor]
        [VectorEditor]
        public RandomVector2 Acceleration { get; set; } = new RandomVector2(new Vector2(0, 100));

        public AccelerationModule() :
            base(ParticleModuleFlag.StartTemplate)
        {

        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Acceleration.Max += Acceleration.Max;

            if(Acceleration.UseRandom)
            {
                template.Acceleration.Min += Acceleration.Min;
                template.Acceleration.UseRandom = true;
            }
        }
    }

    public class DragModule : ParticleModule
    {
        [PropertiesEditor]
        [EditorInfo(ValueEditor.Slider, Format = "{0:0.00}", Min = 0, Max = 2, Step = 0.05)]
        public RandomFloat Coefficient { get; set; } = new RandomFloat( 0.3f );

        public DragModule() :
            base(ParticleModuleFlag.StartTemplate)
        {

        }
        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Drag.Max += Coefficient.Max;

            if(Coefficient.UseRandom)
            {
                template.Drag.Min += Coefficient.Min;
                template.Drag.UseRandom = true;
            }
        }
    }

    public class AccelerationOverLifeModule : ParticleModule
    {
        [PropertiesEditor]
        [VectorEditor]
        public RandomVector2 Acceleration { get; set; } = new RandomVector2(new Vector2(0, 100));

        public AccelerationOverLifeModule() :
            base(ParticleModuleFlag.EndTemplate)
        {
        }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Acceleration.Max += Acceleration.Max;

            if (Acceleration.UseRandom)
            {
                template.Acceleration.Min += Acceleration.Min;
                template.Acceleration.UseRandom = true;
            }
        }
    }
}
