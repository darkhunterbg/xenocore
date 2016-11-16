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

        private List<SoundEffectInstance> effects = new List<SoundEffectInstance>();

        public AudioService(AssetsService assets)
        {
            this.assets = assets;

            SongPlayer = new SongPlayer(MasterChannel);

            EffectChannel = new SoundEffectChannel(MasterChannel);
        }

        public void PlayEffect(SoundEffect effect)
        {
            var instance = effect.CreateInstance();
            effects.Add(instance);
            EffectChannel.AddSound(instance);
            instance.Play();
        }

        public void Dispose()
        {
            foreach (SoundEffectInstance instance in effects)
            {
                instance.Stop();
                instance.Dispose();
            }

            effects.Clear();

            SongPlayer.Stop();
        }

        public void Update(GameTime time)
        {
            int count = effects.Count;
            for (int i = 0; i < count; ++i)
            {
                SoundEffectInstance instance = effects[i];
                if (instance.State == SoundState.Stopped)
                {
                    instance.Dispose();
                    effects.RemoveAt(i);
                    EffectChannel.RemoveSound(instance);
                    --i;
                    --count;
                }
            }
        }
    }
}
