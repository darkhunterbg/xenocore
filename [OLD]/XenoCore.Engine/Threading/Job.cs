using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Threading
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Job
    {
        public Action<Object> Action { get; set; }
        public Object Param { get; set; }

        internal JobTask Task { get; set; }

        public String DebugName { get; private set; }

        //public Job(Object param, Action<Object> action)
        //{
        //    this.Param = param;
        //    this.Action = action;
        //}
        [Conditional("PROFILER")]
        public void SetDebugName(String name)
        {
            DebugName = name;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class JobTask
    {
        static uint internalCounter = 0;

        public uint ID { get; private set; } = ++internalCounter;

        public int CategoryID { get; internal set; }

        internal int RemainingCounter = 0;
    }

}
