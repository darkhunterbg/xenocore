using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Assets;

namespace XenoCore.Engine.Services.Audio
{
    public class SongPlayer
    {
        public SongChannel Channel { get; private set; }

        public MediaState State
        {
            get { return MediaPlayer.State; }
        }

        public Song Current
        {
            get { return MediaPlayer.Queue.ActiveSong; }
        }
        public bool IsMuted
        {
            get { return MediaPlayer.IsMuted; }
            set { MediaPlayer.IsMuted = value; }
        }

        public bool IsShuffled
        {
            get { return MediaPlayer.IsShuffled; }
            set { MediaPlayer.IsShuffled = value; }
        }

        public bool IsRepeating
        {
            get { return MediaPlayer.IsRepeating; }
            set { MediaPlayer.IsRepeating = value; }
        }

        public SongPlayer(AudioChannel parentChannel)
        {
            Channel = new SongChannel(parentChannel);
        }


        public void Play(Song song, TimeSpan? position = null)
        {
            MediaPlayer.Play(song, position);
        }
        public void Pause()
        {
            MediaPlayer.Pause();
        }
        public void Stop()
        {
            MediaPlayer.Stop();
        }
    }
}
