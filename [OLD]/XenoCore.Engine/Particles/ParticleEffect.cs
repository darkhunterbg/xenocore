using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Particles
{
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/BaseClasses/ParticleSystemClass/index.html

    [StructLayout(LayoutKind.Sequential)]
    public class ParticleEffectDescription
    {
        [EditorInfo(ValueEditor.Text)]
        public String Name { get; set; }

        [EditorInfo(ValueEditor.List)]
        public List<ParticleEmitterDescription> Emitters { get; private set; } = new List<ParticleEmitterDescription>();
    }

    [StructLayout(LayoutKind.Sequential)]
    public class ParticleEffect
    {
        public ParticleEffectDescription Description { get; private set; }

        public ParticleEmitter[] Emitters { get; private set; }

        public ParticleEffect(ParticleEffectDescription description)
        {
             this.Description = description;
            Emitters = new ParticleEmitter[description.Emitters.Count];
            for (int i = 0; i < Emitters.Length; ++i)
            {
                Emitters[i] = new ParticleEmitter(description.Emitters[i]);
            }
        }
    }

    
}
