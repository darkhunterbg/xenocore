using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Threading;

namespace XenoCore.Engine.Profiling
{
    [StructLayout(LayoutKind.Sequential)]
    public class ThreadPerformanceData
    {
        public String Name = String.Empty;
        public List<ThreadJobStat> Jobs = new List<ThreadJobStat>();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameData
    {
        public long FrameStart;
        public long FrameEnd;
        public long UpdateStart;
        public long UpdateEnd;
        public long DrawStart;
        public long DrawEnd;
        public TimeSpan GameTime;
        public int FrameIndex;
        public long FrameTime;
        public long UpdateTime;
        public long DrawTime;
        public double FPS;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class ProfilerData
    {
        public FrameData LastFrame;
        public FrameData CollectingFrame;
        public ThreadPerformanceData[] ThreadsData;
    }
}
