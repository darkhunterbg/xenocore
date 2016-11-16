using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Screens;

namespace XenoCore.Engine.World
{
    public class CameraSystem : IDisposable, IDrawableSystem
    {
        public Camera CurrentCamera { get { return Cameras[CurrentCameraIndex]; } }

        public List<Camera> Cameras { get; private set; } = new List<Camera>();

        [ConsoleVariable(Name = "cam_zoom")]
        public float CameraZoom
        {
            get { return CurrentCamera.Zoom; }
            set { CurrentCamera.Zoom = value; }
        }

        [ConsoleVariable(Name = "cam_current")]
        public int CurrentCameraIndex
        {
            get; set;
        }

        public const int DRAW_ORDER = DrawingOrder.GUI - 100;
        public int DrawOrder { get { return DRAW_ORDER; } }

        public CameraSystem()
        {
            Cameras.Add(new Camera(GraphicsService.BackBufferSize.ToVector2()));
            ConsoleService.LoadFromObject(this);
        }

        public void Dispose()
        {
            ConsoleService.UnloadFromObject(this);
        }

        public void SetRenderStateForCurrentCamera()
        {
            CurrentCamera.RenderStateIndex = 1;
            var state = GraphicsService.Renderer.States[CurrentCamera.RenderStateIndex];

            state.TransformMatrix = CurrentCamera.GetMatrix(GraphicsService.BackBufferSize);
            state.Viewport = new Viewport(0,0, GraphicsService.BackBufferSize.X, GraphicsService.BackBufferSize.Y);
        }

        public void Draw(DrawState state)
        {
            foreach (var cam in Cameras)
            {
                if (!cam.Used)
                    continue;

                cam.UpdateViewRect();

                cam.Used = false;
            }
        }
    }
}
