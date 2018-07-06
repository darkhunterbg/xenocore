using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Particles
{
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/BaseClasses/ParticleEmitterClass/index.html

    //public enum EmitterRenderMode
    //{
    //    Normal,
    //}


    [StructLayout(LayoutKind.Sequential)]
    public class ParticleEmitterDescription
    {
        private RequiredModule required;

        [EditorInfo(ValueEditor.Text)]
        public String Name { get; set; }

        //[ColorEditor]
        // public Color DebugColor { get; set; } = Color.White;

        [IntegerEditor(Min = 1, Max = int.MaxValue)]
        public int MaxParticles { get; set; } = 60;

        [JsonIgnore]
        internal ParticleTemplate StartTemplate { get; set; }
        [JsonIgnore]
        internal ParticleTemplate EndTemplate { get; set; }

        [JsonIgnore]
        internal int TotalEmittingParticles { get; set; }

        [JsonIgnore]
        public RequiredModule Required
        {
            get
            {
                if (required == null)
                    required = Modules[0] as RequiredModule;

                return required;
            }

        }
        // private set; } = new RequiredModule();

        // [EditorInfo(ValueEditor.ComboBox)]
        // public EmitterRenderMode RenderMode { get; set; }

        [EditorInfo(ValueEditor.List)]
        public List<ParticleModule> Modules { get; private set; } = new List<ParticleModule>();

        [JsonConstructor]
        internal ParticleEmitterDescription()
        {

        }


        public ParticleEmitterDescription(String name)
        {
            this.Name = name;
            Modules.Add(new RequiredModule());
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public class ParticleEmitter
    {
        internal float _duration = 0;
        internal float _delay = 0;
        internal int spawned = 0;

        public ParticleEmitterDescription Description { get; private set; }
        internal Object[] ModuleData { get; private set; }
        public ListArray<Particle> Particles { get; private set; }


        public ParticleEmitter(ParticleEmitterDescription description)
        {
            var count = Math.Max(description.MaxParticles, 1);
            this.Description = description;
            Particles = new ListArray<Particle>(count);
            ModuleData = new object[description.Modules.Count];
            for (int i = 0; i < ModuleData.Length; ++i)
            {
                ModuleData[i] = description.Modules[i].NewModuleData();
            }
        }
    }

}
