using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XenoCore.Engine.Threading;

namespace XenoCore.Desktop.Threading
{
    public abstract class ThreadProvider : IThreadProvider
    {
        public void StartThread( Action work)
        {
            new Thread(() =>
            {
                work();
            }).Start();
        }

        public abstract void LockCurrentThreadToCore(int core);
        public abstract void UnlockCurrentThread();
        public abstract int GetCurrentCore();
        public abstract int GetCurrentThreadID();
        public abstract int[] GetAvailableCores(bool includeHyperThreads);
    }
}
