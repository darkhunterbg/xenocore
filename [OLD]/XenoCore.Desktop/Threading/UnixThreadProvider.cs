using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XenoCore.Engine.Threading;

namespace XenoCore.Desktop.Threading
{
    public class UnixThreadProvider : ThreadProvider
    {
        [DllImport("libc.so.6", SetLastError = true)]
        private static extern int sched_setaffinity(int pid, IntPtr cpusetsize, ref ulong cpuset);

        [DllImport("libc.so.6", SetLastError = true)]
        private static extern int sched_getcpu();

        [DllImport("libpthread.so.0", SetLastError = true)]
        private static extern int pthread_self();

        public override void LockCurrentThreadToCore(int core)
        {
            ulong processorMask = 1UL << core;
            SetAffinity(processorMask);
        }

        public override void UnlockCurrentThread()
        {
            ulong processorMask = 0;
            for (int i = 0; i < Environment.ProcessorCount; ++i)
                processorMask |= 1UL << i;

            SetAffinity(processorMask);
        }

        private static void SetAffinity(ulong processorMask)
        {
            int result = sched_setaffinity(0, new IntPtr(sizeof(ulong)), ref processorMask);
            if (result == -1)
                throw new Exception("Failed to set thread affinity!");
        }

        public override int GetCurrentCore()
        {
            return sched_getcpu();
        }

        public override int GetCurrentThreadID()
        {
            return pthread_self();
        }

        public override int[] GetAvailableCores(bool includeHyperThreads)
        {
#warning TODO
            int[] result = new int[Environment.ProcessorCount];
            for (int i = 0; i < result.Length; ++i)
                result[i] = i;
            return result;
        }
    }
}
