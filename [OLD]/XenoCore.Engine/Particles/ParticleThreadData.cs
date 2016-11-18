using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Particles
{
    class ParticleThreadData
    {
        public Random Random = new Random();
        public ParticleOperatorContext OperatorContext = new ParticleOperatorContext();
        public ParticleSpawnContext SpawnContext = new ParticleSpawnContext();
        public ParticleResetContext ResetContext = new ParticleResetContext();
    }
}
