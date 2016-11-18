using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Physics
{
    class PhysicsThreadData
    {
        public List<CollisionComponent> BroadPhaseResults = new List<CollisionComponent>();
        public List<CollisionComponent> TreeList = new List<CollisionComponent>();
    }
}
