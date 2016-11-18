using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;
using XenoCore.Engine.Resources;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Particles
{
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/Modules/Color/index.html

    [StructLayout(LayoutKind.Sequential)]
    public class ColorInitModule : ParticleModule
    {
        [PropertiesEditor]
        [ColorEditor]
        public RandomColor StartColor { get; set; } = new RandomColor(Color.Black, Color.White, false);

        [PropertiesEditor]
        [AlphaEditor]
        public RandomFloat StartAlpha { get; set; } = new RandomFloat(1.0f);

        public ColorInitModule() : base(ParticleModuleFlag.StartTemplate) { }


        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Alpha.Max = StartAlpha.Max;
            if (StartAlpha.UseRandom)
            {
                template.Alpha.Min = StartAlpha.Min;
                template.Alpha.UseRandom = true;
            }

            template.Color.Max = StartColor.Max;
            if (StartColor.UseRandom)
            {
                template.Color.Min = StartColor.Min;
                template.Color.UseRandom = true;
            }
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public class ColorOverLifeModule : ParticleModule
    {
 
        [PropertiesEditor]
        [ColorEditor]
        public RandomColor ColorOverLife { get; set; } = new RandomColor(Color.Black, Color.White, false);

        [PropertiesEditor]
        [AlphaEditor]
        public RandomFloat AlphaOverLife { get; set; } = new RandomFloat(1.0f);

        public ColorOverLifeModule() : base(ParticleModuleFlag.EndTemplate ) { }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            template.Alpha.Max = AlphaOverLife.Max;
            if (AlphaOverLife.UseRandom)
            {
                template.Alpha.Min = AlphaOverLife.Min;
                template.Alpha.UseRandom = true;
            }

            template.Color.Max = ColorOverLife.Max;
            if (ColorOverLife.UseRandom)
            {
                template.Color.Min = ColorOverLife.Min;
                template.Color.UseRandom = true;
            }
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public class ScaleColorLifeModule : ParticleModule
    {
        [ColorEditor]
        public Color ColorScaleOverLife { get; set; } = Color.White;

        [AlphaEditor(Max = 2.0f)]
        public float AlphaScaleOverLife { get; set; } = 1.0f;

        [BooleanEditor]
        public bool EmitterTime { get; set; } = false;

        public ScaleColorLifeModule() : base(ParticleModuleFlag.EndTemplate) { }

        public override void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context)
        {
            var c = ColorScaleOverLife.ToVector3();
            template.Color.Min = new Color(template.Color.Min.ToVector3() * c);
            template.Color.Max = new Color(template.Color.Max.ToVector3() * c);

            template.Alpha.Min *= AlphaScaleOverLife;
            template.Alpha.Max *= AlphaScaleOverLife;

        }

     
    }
}
