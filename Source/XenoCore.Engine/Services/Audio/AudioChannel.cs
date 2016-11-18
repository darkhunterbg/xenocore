using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Services.Audio
{
    public class AudioChannel
    {
        private float localVolume = 1.0f;
        private float scaleVolume  = 1.0f;
        private readonly List<AudioChannel> ChildrenChannels = new List<AudioChannel>();

        public String Name { get; set; }

        public float TotalVolume { get { return scaleVolume * Volume; } }

        public float Volume
        {
            get
            {
                return localVolume;
            }
            set
            {
                localVolume = MathHelper.Clamp(value, 0, 1);
                OnVolumeChanged();


            }
        }
        public AudioChannel ParentChannel { get; private set; }


        public AudioChannel(AudioChannel parent = null)
        {
            ParentChannel = parent;
            if (parent != null)
                parent.ChildrenChannels.Add(this);
        }

        protected virtual void OnVolumeChanged()
        {
            foreach (AudioChannel channel in ChildrenChannels)
            {
                channel.scaleVolume = TotalVolume;
                channel.OnVolumeChanged();
            }
        }
    }

    public class SongChannel : AudioChannel
    {
        protected override void OnVolumeChanged()
        {
            MediaPlayer.Volume = TotalVolume;
            base.OnVolumeChanged();
        }

        public SongChannel(AudioChannel parent = null)
            : base(parent)
        {
            MediaPlayer.Volume = TotalVolume;
        }

    }

    public class SoundEffectChannel : AudioChannel
    {
        private List<SoundEffectEntry> sounds = new List<SoundEffectEntry>();

        public SoundEffectChannel(AudioChannel parent = null)
            : base(parent)
        {
        }

        internal void AddSound(SoundEffectEntry instance)
        {
            sounds.Add(instance);
        }
        internal void RemoveSound(SoundEffectEntry instance)
        {
            sounds.Remove(instance);
        }

        protected override void OnVolumeChanged()
        {
            foreach (SoundEffectEntry instance in sounds)
                instance.UpdateVolume();
            base.OnVolumeChanged();
        }
    }

}
