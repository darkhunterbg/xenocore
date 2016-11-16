using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Profiling
{
    public interface IProfilerViewer
    {
        void Update(ProfilerData data);
        void RenderData(ProfilerRenderer renderer, ProfilerData data);
    }
}
