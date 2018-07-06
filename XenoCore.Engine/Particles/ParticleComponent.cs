using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;

namespace XenoCore.Engine.Particles
{
    public class ParticleComponent : Component
    {
        public ParticleEffect Effect { get; internal set; }

        internal bool _updated;

        public void Reset()
        {
            Effect = null;
        }
    }
}
