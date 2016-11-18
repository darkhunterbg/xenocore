using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.World
{
    public class Camera
    {
        public Vector2 Position;
        public Vector2 WorldViewport;

        public float Zoom = 1.0f;

        public RectangleF ViewRect;

        public int RenderStateIndex;

        public bool Used { get; set; }
       
        public Camera(Vector2 worldViewport,float zoom = 1.0f)
        {
            this.WorldViewport = worldViewport;
            this.Zoom = zoom;
        }

        public void UpdateViewRect()
        {
            ViewRect.Size = WorldViewport / Zoom;
            ViewRect.Center = Position;
        }
        public Matrix GetMatrix(Point ViewportSize)
        {
            Vector2 scale = (ViewportSize.ToVector2() * Zoom) / (WorldViewport );
            return Matrix.CreateScale(new Vector3(scale, 1));
        }
    }
}
