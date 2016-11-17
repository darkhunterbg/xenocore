using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Systems.Entities;

namespace XenoCore.Engine.Systems.Animation
{
    public class AnimationComponent : Component
    {
        public int FrameIndex;
        public SpriteAnimation Animation;

        public float FrameElapsedTime;

        public bool Paused;
        public float AnimationSpeed;

        internal void Reset()
        {
            FrameIndex = 0;
            Animation = null;
            FrameElapsedTime = 0;
            Paused = false;
            AnimationSpeed = 1.0f;
        }

        public void SetAnimation(SpriteAnimation animation, bool restartIfSame = false)
        {
            if(Animation != animation || restartIfSame)
            {
                Animation = animation;
                FrameElapsedTime = 0;
                FrameIndex = 0;
            }
        }

    }
}
