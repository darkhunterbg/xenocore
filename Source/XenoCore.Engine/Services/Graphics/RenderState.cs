using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Services.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public class RenderState
    {
        public Matrix TransformMatrix;
        //public Viewport Viewport;
    }
}
