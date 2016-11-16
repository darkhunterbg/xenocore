using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Threading
{
    [StructLayout(LayoutKind.Sequential)]
    public class ThreadData
    {
        public ThreadJobStat[] JobStats = new ThreadJobStat[1024 * 8];

        public int JobStatsCount = 0;

        public String Name { get; set; }
        public int Core { get; internal set; }
        public bool IsRunning { get; internal set; }
        public bool IsSleeping { get; internal set; }

        public int ThreadID { get; internal set; } = -1;
        public int ThreadIndex { get; internal set; }

        internal Stack<Job> JobStack = new Stack<Job>();

        internal int StackCounter = 0;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ThreadJobStat
    {
        public long StartTime;
        public long EndTime;
        public int Stack;
        public Job Job;
    }


}
