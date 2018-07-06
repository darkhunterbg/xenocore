using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Scripting
{
   public class Timer
    {
        public float Duration;

        internal float _remaining;

        public bool Repeat;

        internal TimingComponent _component;

        public uint ID;
    }
}
