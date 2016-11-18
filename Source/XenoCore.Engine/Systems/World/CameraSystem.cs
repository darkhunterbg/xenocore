using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Graphics;

namespace XenoCore.Engine.Systems.World
{
    public class Camera
    {
        public Vector2 Position;
        public Vector2 WorldViewportSize;

        public float Zoom = 1.0f;

        public RectangleF ViewRect;

        public Camera(Vector2 worldViewportSize, float zoom = 1.0f)
        {
            WorldViewportSize = worldViewportSize;
            Zoom = zoom;
        }

        public void UpdateViewRect()
        {
            ViewRect.Size = WorldViewportSize / Zoom;
            ViewRect.Center = Position;
        }
        public Matrix GetMatrix(Point ViewportSize)
        {
            Vector2 scale = (ViewportSize.ToVector2() * Zoom) / (WorldViewportSize);
            return Matrix.CreateScale(new Vector3(scale, 1));
        }
    }


    public class CameraSystem : IDrawableSystem
    {
        private readonly GraphicsService GraphicsService;

        public Camera CurrentCamera { get; set; }

        public int DrawOrder { get { return -2; } }

        public CameraSystem()
        {
            GraphicsService = ServiceProvider.Get<GraphicsService>();

            CurrentCamera = new Camera(GraphicsService.ViewportSize.ToVector2());
        }

        public void Draw(DrawState state)
        {
            if (CurrentCamera != null)
            {
                CurrentCamera?.UpdateViewRect();
                GraphicsService.Renderer.States[1].TransformMatrix = CurrentCamera.GetMatrix(GraphicsService.ViewportSize);
            }
        }
    }
}
