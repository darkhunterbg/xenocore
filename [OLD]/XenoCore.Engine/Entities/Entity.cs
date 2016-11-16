using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public class Entity
    {
        public uint ParentID { get; internal set; }
        public uint ID { get; internal set; }
        public List<uint> Children { get; private set; } = new List<uint>();
        public List<Component> Components { get; private set; } = new List<Component>();
        public uint ChildDepth { get; internal set; } = 0;

        public bool Used { get; internal set; } = false;
    }

 
}
