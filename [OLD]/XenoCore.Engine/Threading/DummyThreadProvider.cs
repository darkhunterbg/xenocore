using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Threading
{
    public class DummyThreadProvider : IThreadProvider
    {
        public int[] GetAvailableCores(bool includeHyperThreads)
        {
            return new int[] { 1 };
        }

        public int GetCurrentCore()
        {
            return 1;
        }

        public int GetCurrentThreadID()
        {
            return 1;
        }

        public void LockCurrentThreadToCore(int core)
        {
          
        }

        public void StartThread(Action work)
        {
           
        }

        public void UnlockCurrentThread()
        {
        }
    }
}
