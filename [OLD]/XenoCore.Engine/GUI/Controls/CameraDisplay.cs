using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.World;

namespace XenoCore.Engine.GUI
{
    public enum CameraUpdateType
    {
        Fit,
        None,

    }


    public class CameraDisplay : GUIControl
    {
        public Camera Camera { get; set; }

        public CameraUpdateType UpdateCameraAction { get; set; }

        public override void Update(GUISystemState systemState)
        {
            if (Camera != null)
            {

                if (systemState.CameraRenderStateIndex >= GraphicsService.Renderer.States.Length)
                    throw new Exception("Used too much camera displays!");

                var state = GraphicsService.Renderer.States[systemState.CameraRenderStateIndex];

                Camera.RenderStateIndex = systemState.CameraRenderStateIndex;

                Camera.Used = true;

                switch (UpdateCameraAction)
                {
                    case CameraUpdateType.None:
                        break;
                    case CameraUpdateType.Fit:
                        Camera.WorldViewport = GraphicsService.WindowSize.ToVector2();// new Vector2(controlState.Bounds.Width, controlState.Bounds.Height);
                        break;
                }

                // var originalSize = Camera.WorldViewport;

                state.TransformMatrix = Camera.GetMatrix(new Point(State.Bounds.Width, State.Bounds.Height));
                state.Viewport = new Viewport(State.Bounds.X, State.Bounds.Y, State.Bounds.Width, State.Bounds.Height);

                //  Camera.WorldViewport = originalSize;

                ++systemState.CameraRenderStateIndex;

            }
        }
    }
}
