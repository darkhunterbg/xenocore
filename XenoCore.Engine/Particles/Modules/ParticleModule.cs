using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Resources;

namespace XenoCore.Engine.Particles
{
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/BaseClasses/ParticleModule/index.html
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/

 
    public enum ParticleModuleFlag
    {
        None = 0,
        StartTemplate = 0x1,
        Initializer = 0x2,
        Spawner = 0x4,
        EndTemplate = 0x8,
        Reset = 0x10,
        Updater = 0x20,
    }

    [StructLayout(LayoutKind.Sequential)]
    public abstract class ParticleModule
    {

        [Newtonsoft.Json.JsonIgnore]
        public ParticleModuleFlag Flags { get; private set; }

        public ParticleModule(ParticleModuleFlag flags)
        {
            this.Flags = flags;
        }
        public virtual Object NewModuleData()
        {
            return null;
        }
        public virtual void ApplyToTemplate(ParticleTemplate template, ParticleTemplateContext context) { }
        public virtual void ApplyOperator(Particle particle, ParticleOperatorContext context, float lerp) { }
        public virtual void SpawnParticles(ParticleSpawnContext context, int count) { }
        public virtual int GetNewParticlesCount(ParticleSpawnContext context) { return 0; }
        public virtual void InitializeParticle(Particle particle, int spawnIndex, ParticleEmitterDescription emitter) { }
        public virtual void Reset(ParticleResetContext context) { }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class RequiredModule : ParticleModule
    {
        public RequiredModule() :
            base(ParticleModuleFlag.None)
        {
        }

        [TextureEditor]
        public String TextureName { get; set; }
  
        [Newtonsoft.Json.JsonIgnore]
        public Texture Texture
        {
            get; set;
        } = new Texture() { id = 1 };

        public Rectangle TexturePart
        {
            get;
            set;
        }

        [TimeEditor]
        public float Duration { get; set; } = 1.0f;

        [EnumEditor]
        public BlendingMode Blending { get; set; } = BlendingMode.Alpha;

        [TimeEditor]
        public float Delay { get; set; } = 0;


    }

    class RandomSeedData
    {
        public int Seed;
    }
}
