using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XenoCore.Engine.Threading;

namespace XenoCore.Desktop.Threading
{
    public class Win32ThreadProvider : ThreadProvider
    {
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentProcessorNumber();

        private static ProcessThread GetProcessThread()
        {
            int id = GetCurrentThreadId();

            var process = Process.GetCurrentProcess();

            var threads = process.Threads;

            for (int i = 0; i < threads.Count; ++i)
            {
                var thread = threads[i];
                if (thread.Id == id)
                {
                    return thread;
                }
            }
            return null;
        }

        public override void LockCurrentThreadToCore(int core)
        {
            Thread.BeginThreadAffinity();

            var thread = GetProcessThread();
            thread.IdealProcessor = core;
            thread.ProcessorAffinity = new IntPtr(1 << core);
        }
        public override void UnlockCurrentThread()
        {
            var thread = GetProcessThread();
            thread.ProcessorAffinity = new IntPtr(1 << 32);

            Thread.EndThreadAffinity();
        }
        public override int GetCurrentCore()
        {
            return GetCurrentProcessorNumber();
        }
        public override int GetCurrentThreadID()
        {
            return GetCurrentThreadId();
        }
        public override int[] GetAvailableCores(bool includeHyperThreads)
        {
            var processors = Win32Processor.GetProcessorInformation();

            List<int> result = new List<int>();

            int i = 0;
            foreach (var p in processors)
            {
                foreach (var c in p.Cores)
                {
                    if (includeHyperThreads)
                    {
                        for (int j = 0; j < c.Threads; ++j)
                            result.Add(i + j);
                    }
                    else
                        result.Add(i);

                    i += c.Threads;

                }
            }

            return result.ToArray();

        }
    }
}
