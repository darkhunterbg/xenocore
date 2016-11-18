using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Desktop.Threading
{
    class ProcessorCore
    {
        public int Threads { get; set; }
    }

    class PhysicalProcessor
    {
        public List<ProcessorCore> Cores { get; private set; } = new List<ProcessorCore>();
    }

    [StructLayout(LayoutKind.Sequential)]
    struct CACHE_DESCRIPTOR
    {
        public CACHE_LEVEL Level;
        public byte Associativity;
        public ushort LineSize;
        public uint Size;
        public PROCESSOR_CACHE_TYPE Type;
    }

    enum CACHE_LEVEL : byte
    {
        L1 = 1,
        L2 = 2,
        L3 = 3,
    }

    enum PROCESSOR_CACHE_TYPE
    {
        Unified = 0,
        Instruction = 1,
        Data = 2,
        Trace = 3,
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION
    {
        public UIntPtr ProcessorMask;
        public LOGICAL_PROCESSOR_RELATIONSHIP Relationship;
        public ProcessorRelationUnion RelationUnion;
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ProcessorRelationUnion
    {
        [FieldOffset(0)]
        public CACHE_DESCRIPTOR Cache;
        [FieldOffset(0)]
        public uint NumaNodeNumber;
        [FieldOffset(0)]
        public byte ProcessorCoreFlags;
        [FieldOffset(0)]
        private UInt64 Reserved1;
        [FieldOffset(8)]
        private UInt64 Reserved2;
    }

    enum LOGICAL_PROCESSOR_RELATIONSHIP : uint
    {
        ProcessorCore = 0,
        NumaNode = 1,
        RelationCache = 2,
        ProcessorPackage = 3,
    }



    static class Win32Processor
    {

        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private const int CORE_HYPERTHREADING = 1;

        [DllImport(@"kernel32.dll", SetLastError = true)]
        private static extern bool GetLogicalProcessorInformation(IntPtr Buffer, ref uint ReturnLength);

        public static List<PhysicalProcessor> GetProcessorInformation()
        {
            uint ReturnLength = 0;
            GetLogicalProcessorInformation(IntPtr.Zero, ref ReturnLength);
            if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
            {
                IntPtr Ptr = Marshal.AllocHGlobal((int)ReturnLength);
                try
                {
                    if (GetLogicalProcessorInformation(Ptr, ref ReturnLength))
                    {
                        int size = Marshal.SizeOf(typeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
                        int len = (int)ReturnLength / size;
                        SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] Buffer = new SYSTEM_LOGICAL_PROCESSOR_INFORMATION[len];
                        IntPtr Item = Ptr;
                        for (int i = 0; i < len; i++)
                        {
                            Buffer[i] = (SYSTEM_LOGICAL_PROCESSOR_INFORMATION)Marshal.PtrToStructure(Item, typeof(SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
                            Item += size;
                        }
                        return GetProcessorsInfo(Buffer);

                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(Ptr);
                }
            }
            return new List<PhysicalProcessor>();

        }

        private static List<PhysicalProcessor> GetProcessorsInfo(SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] info)
        {
            List<PhysicalProcessor> result = new List<PhysicalProcessor>();

            var processors = info.Where(p => p.Relationship == LOGICAL_PROCESSOR_RELATIONSHIP.ProcessorPackage).ToList();

            foreach (var p in processors)
            {
                PhysicalProcessor processor = new PhysicalProcessor();
                var components = info.Where(q => (q.ProcessorMask.ToUInt64() & p.ProcessorMask.ToUInt64()) > 0);
                var cores = components.Where(q => q.Relationship == LOGICAL_PROCESSOR_RELATIONSHIP.ProcessorCore);

                foreach (var c in cores)
                {
                    ProcessorCore core = new ProcessorCore();
                    core.Threads = ((c.RelationUnion.ProcessorCoreFlags & CORE_HYPERTHREADING) > 0) ? 2 : 1;
                    processor.Cores.Add(core);
                }


                result.Add(processor);
            }

            return result;
        }

    }
}
