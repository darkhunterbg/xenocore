using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Editor;

namespace XenoCore.Engine.Particles
{
    //https://docs.unrealengine.com/latest/INT/Engine/Rendering/ParticleSystems/Reference/Modules/Required/index.html


    [StructLayout(LayoutKind.Sequential)]
    internal class SpawnConiniouslyData
    {
        public int LastEmitted;
    }


    [StructLayout(LayoutKind.Sequential)]
    public class SpawnContiniouslyModule : ParticleModule
    {
        private float spawn = 10;

        [FloatEditor(Min = 0, Max = 1000)]
        public float SpawnRate
        {
            get { return spawn; }
            set
            {
                spawn = value;
                TimePerSpawn = 1.0f / SpawnRate;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public float TimePerSpawn { get; private set; }

        public SpawnContiniouslyModule() :
            base(ParticleModuleFlag.Spawner | ParticleModuleFlag.Reset)
        {
            SpawnRate = spawn;
        }
        public override object NewModuleData()
        {
            return new SpawnConiniouslyData();
        }

        public override int GetNewParticlesCount(ParticleSpawnContext context)
        {
            var data = context.Data as SpawnConiniouslyData;
            int emit = (int)(decimal)(context.EmitterTime / TimePerSpawn)  - data.LastEmitted;
            data.LastEmitted += emit;

            return emit;
        }
        public override void SpawnParticles(ParticleSpawnContext context,int emit)
        {
            var data = context.Data as SpawnConiniouslyData;

            float offset = context.EmitterTime - data.LastEmitted * TimePerSpawn;

            for (int i = 0; i < emit; ++i)
            {
                var particle = context.SpawnParticle();
                particle.ElaspedLife += offset + TimePerSpawn * i;
            }
             
        }

        public override void Reset(ParticleResetContext context)
        {
            var data = context.Data as SpawnConiniouslyData;
            data.LastEmitted = 0;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    internal class SpawnBurstData
    {
        public bool IsActivated;
    }


    [StructLayout(LayoutKind.Sequential)]
    public class SpawnBurstModule : ParticleModule
    {
        [DistributionEditor]
        public float BurstTime { get; set; } = 0.0f;

        [IntegerEditor(Min = 1, Max = 1000)]
        public int BurstCount { get; set; } = 10;


        public SpawnBurstModule() :
            base(ParticleModuleFlag.Spawner | ParticleModuleFlag.Reset)
        { }

        public override object NewModuleData()
        {
            return new SpawnBurstData();
        }


        public override int GetNewParticlesCount(ParticleSpawnContext context)
        {
            var data = context.Data as SpawnBurstData;

            int emitCount = 0;

            if (!data.IsActivated)
            {
                float burstTime = BurstTime * context.Emitter.Description.Required.Duration;
                if (context.EmitterTime >= burstTime)
                {
                    emitCount += BurstCount;
                    data.IsActivated = true;
                }
            }

   
            return emitCount;
        }

        public override void SpawnParticles(ParticleSpawnContext context, int emitCount)
        {
            var data = context.Data as SpawnBurstData;

            float burstTime = BurstTime * context.Emitter.Description.Required.Duration;

            for (int i = 0; i < emitCount; ++i)
            {
                var p = context.SpawnParticle();
                p.ElaspedLife += context.EmitterTime - burstTime;
            }

          
        }


        public override void Reset(ParticleResetContext context)
        {
            var data = context.Data as SpawnBurstData;
            data.IsActivated = false;
        }
    }



}
