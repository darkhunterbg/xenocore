using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Graphics;

namespace XenoCore.Engine.Systems.Animation
{
    public class Sprite
    {
        public Rectangle Region;

        public Sprite(Rectangle region)
        {
            this.Region = region;
        }
    }

    public class SpriteSheet
    {
        public Texture Texture { get; private set; }
        public List<Sprite> Sprites { get; private set; } = new List<Sprite>();

        public SpriteSheet(Texture texture)
        {
            this.Texture = texture;
        }
    }

    public class SpriteAnimationFrame
    {
        public Sprite Sprite { get; private set; }
        public float Duration { get; set; }

        public SpriteAnimationFrame(Sprite sprite,float duration)
        {
            Sprite = sprite;
            Duration = duration;
        }
    }

    public class SpriteAnimation
    {
        public String Name { get; set; }

        public bool Loop { get; set; }

        public SpriteSheet SpriteSheet { get; private set; }
        public List<SpriteAnimationFrame> Frames { get; private set; } = new List<SpriteAnimationFrame>();

        public SpriteAnimation(String name, SpriteSheet sheet)
        {
            Name = name;
            SpriteSheet = sheet;
        }
    }
}
