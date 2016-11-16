using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Threading
{
    public interface IThreadProvider
    {
        void LockCurrentThreadToCore(int core);
        void UnlockCurrentThread();

        void StartThread(Action work);

        int GetCurrentCore();
        int GetCurrentThreadID();

        int[] GetAvailableCores(bool includeHyperThreads);
    }
}
