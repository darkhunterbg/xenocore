using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;

namespace XenoCore.Engine.Scripting
{
    public class TimingComponent :Component
    {
        internal Timer _timer;

        public float Duration { get
            {
                return _timer.Duration;
            }
            set
            {
                _timer.Duration = value;
                _timer._remaining = _timer.Duration;
            }
        }
        public bool Repeat
        {
            get { return _timer.Repeat; }
            set { _timer.Repeat = value; }
        }

        public uint TimerID
        {
            get { return _timer.ID; }
        }

        public Action Callback;

        internal void Reset()
        {
            _timer = null;
        }
    }
}
