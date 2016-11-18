using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Particles
{
    [StructLayout(LayoutKind.Sequential)]
    public class Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;

        public float StartLife;
        public float ElaspedLife;

        public bool New;

        public byte Seed;

        public void Reset()
        {
            Position = Velocity = Acceleration = Vector2.Zero;
            ElaspedLife = StartLife = 0;
        }

        public void LoadTemplate(ParticleTemplate p, byte seed)
        {
            Velocity = p.Velocity.Get(seed);
            Acceleration = p.Acceleration.Get(seed);

            ElaspedLife = 0;
            StartLife = p.Life.Get(seed);

            Seed = seed;
        }
    }

  
    [StructLayout(LayoutKind.Sequential)]
    public class ParticleTemplate
    {
        public RandomVector2 Velocity = new RandomVector2(Vector2.Zero);
        public RandomVector2 Acceleration = new RandomVector2(Vector2.Zero);
        public RandomVector2 Size = new RandomVector2(Vector2.One);
        public RandomColor Color = new RandomColor(Microsoft.Xna.Framework.Color.White);
        public RandomFloat Alpha = new RandomFloat(1.0f);
        public RandomFloat Life = new RandomFloat(0.0f);
        public RandomFloat Rotation = new RandomFloat(0);
        public RandomFloat RotationRate = new RandomFloat(0);
        public RandomFloat Drag = new RandomFloat(0);

        public void Reseed(Random random)
        {
            Velocity.Seed = random.NextByte();
            Acceleration.Seed = random.NextByte();
            Size.Seed = random.NextByte();
            Color.Seed = random.NextByte();
            Alpha.Seed = random.NextByte();
            Life.Seed = random.NextByte();
            Rotation.Seed = random.NextByte();
            RotationRate.Seed = random.NextByte();
            Drag.Seed = random.NextByte();
        }
    }

    public class ParticleTemplateContext
    {
        public ParticleEmitterDescription Emitter;
        public Object Data;
        public Random Random;

    }

    [StructLayout(LayoutKind.Sequential)]
    public class ParticleState
    {
        public Vector2 Position;
        public Vector2 Size;
        public Color Color;
        public float Alpha;
        public float Rotation;
        public float RotationRate;

        public void Reset()
        {
            Position = Vector2.Zero;
            Size = Vector2.One;
            Color = Color.White;
            Alpha = 1.0f;
            Rotation = 0;
            RotationRate = 0;
        }
        public void Load(ParticleTemplate startTemplate, ParticleTemplate endTempalte, byte seed, float lerp)
        {
            Size = Vector2.Lerp(startTemplate.Size.Get(seed), endTempalte.Size.Get(seed), lerp);
            Color = Color.Lerp( startTemplate.Color.Get(seed), endTempalte.Color.Get(seed), lerp);
            Alpha = MathHelper.Lerp( startTemplate.Alpha.Get(seed), endTempalte.Alpha.Get(seed), lerp);
            Rotation = MathHelper.Lerp(startTemplate.Rotation.Get(seed), endTempalte.Rotation.Get(seed), lerp);
            RotationRate = MathHelper.Lerp(startTemplate.RotationRate.Get(seed), endTempalte.RotationRate.Get(seed), lerp);
        }
    }


    public class ParticleOperatorContext
    {
        public ParticleTemplate Template { get; internal set; }
        public ParticleState State { get; private set; } = new ParticleState();
        public float Lerp { get; internal set; }
        public Object Data { get; internal set; }
    }

    public class ParticleSpawnContext
    {
        public float EmitterTime { get; internal set; }
        public Object Data { get; internal set; }
        public ParticleEmitter Emitter { get; internal set; }
        internal ParticleSystem system;
        internal Vector2 emitterPosition;

        public Particle SpawnParticle()
        {
            return system.EmitParticle(Emitter, ref emitterPosition);
        }
    }

    public class ParticleResetContext
    {
        public Object Data { get; internal set; }
        public Random Random { get; internal set; }
    }

}
