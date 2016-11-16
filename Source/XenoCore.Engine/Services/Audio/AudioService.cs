using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using XenoCore.Engine.Services.Assets;

namespace XenoCore.Engine.Services.Audio
{
    public class AudioService : IDisposable
    {
        private AssetsService assets;

        public SongPlayer SongPlayer { get; private set; }
        public AudioChannel MasterChannel { get; private set; } = new AudioChannel();
        public SoundEffectChannel EffectChannel { get; private set; }

        private ListArray<SoundEffectEntry> effects = new ListArray<SoundEffectEntry>(1024);

        public AudioService(AssetsService assets)
        {
            this.assets = assets;

            SongPlayer = new SongPlayer(MasterChannel);

            EffectChannel = new SoundEffectChannel(MasterChannel);
        }

        public SoundEffectEntry NewEffect(SoundEffect effect)
        {
            SoundEffectEntry e = effects.New();
            e.LoadEffect(effect, EffectChannel);
            return e;
        }

        public void Dispose()
        {
            foreach (SoundEffectEntry effect in effects)
            {
                effect.Stop();
                effect.Dispose();
            }

            effects.Clear();

            SongPlayer.Stop();
        }

        public void Update(GameTime time)
        {
            //int count = effects.Count;
            //for (int i = 0; i < count; ++i)
            //{
            //    SoundEffectInstance instance = effects[i];
            //    if (instance.State == SoundState.Stopped)
            //    {
            //        instance.Dispose();
            //        effects.RemoveAt(i);
            //        EffectChannel.RemoveSound(instance);
            //        --i;
            //        --count;
            //    }
            //}
        }
    }
}
