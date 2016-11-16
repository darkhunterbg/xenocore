using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.World;

namespace XenoCore.Engine.Physics
{
    public class PhysicsDebugger : IDrawableSystem, IDisposable
    {
        public const int DRAW_ORDER = DrawingOrder.WORLD + 1000;
        public int DrawOrder { get { return DRAW_ORDER; } }

        private CameraSystem cameraSystem;
        private PhysicsSystem physicsSystem;
        private WorldSystem worldSystem;

        [ConsoleVariable( Name ="phys_debug")]
        public bool Enabled { get;  set; }

        public PhysicsDebugger(CameraSystem cameraSystem, PhysicsSystem physicsSystem, WorldSystem worldSystem)
        {
            this.cameraSystem = cameraSystem;
            this.physicsSystem = physicsSystem;
            this.worldSystem = worldSystem;

            ConsoleService.LoadFromObject(this);
        }

        public void Draw(DrawState state)
        {
            if (!Enabled)
                return;

            RectangleF rect = new RectangleF();

            foreach (var component in physicsSystem.CollisionComponents)
            {
                var worldComponent = worldSystem.GetComponent(component.Entity.ID);

                Color c = Color.Black;

                DynamicComponent dynamic = physicsSystem.GetDynamicComponent(component.Entity.ID);
                if (dynamic != null)
                {
                    c = dynamic.IsAwake ? Color.LightGreen : Color.Blue;
                }

                foreach (var s in component.Shapes)
                {
                    var shape = s as BoxShape;
                    rect = shape.bb;
                    rect.Center += worldComponent.Position;

                    foreach (var camera in cameraSystem.Cameras)
                    {
                        if (!camera.Used)
                            continue;

                        GraphicsRendererExtender.RenderBox(camera, ref rect, c, 1, true);
                    }
                }
            }
        }

        public void Dispose()
        {
            ConsoleService.UnloadFromObject(this);
        }
    }
}
