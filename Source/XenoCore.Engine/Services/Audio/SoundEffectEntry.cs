using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Services.Audio
{
    public class SoundEffectEntry
    {
        private SoundEffectChannel channel;
        private SoundEffectInstance instance;
        private float volume = 1.0f;

        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = MathHelper.Clamp(volume, 0, 1);
                UpdateVolume();
            }
        }
        public float Pitch
        {
            get { return instance.Pitch; }
            set { instance.Pitch = value; }
        }
        public float Pan
        {
            get { return instance.Pan; }
            set { instance.Pan = value; }
        }
        public bool Loop
        {
            get { return instance.IsLooped; }
            set { instance.IsLooped = true; }
        }
        public SoundState State
        {
            get { return instance.State; }
        }

        public SoundEffect Effect { get; private set; }

        public SoundEffectEntry() { }

        public void LoadEffect(SoundEffect effect, SoundEffectChannel channel)
        {
            volume = 1.0f;
            instance = effect.CreateInstance();
            this.channel = channel;
            Effect = effect;

            channel.AddSound(this);

            UpdateVolume();
        }

        internal void UpdateVolume()
        {
            instance.Volume = volume * channel.TotalVolume;
        }

        public void Play()
        {
            instance.Play();
        }
        public void Pause()
        {
            instance.Pause();
        }
        public void Stop()
        {
            instance.Stop();
        }
        public void Resume()
        {
            instance.Resume();
        }

     

        internal void Dispose()
        {
            channel.RemoveSound(this);
            instance.Dispose();
            instance = null;
        }

    }
}
