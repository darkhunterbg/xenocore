using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Graphics
{
    public class GraphicsService
    {
        private static GraphicsService instance;

        private GraphicsDevice device;

        private GraphicsResourceStorage resourceCache;
        private GraphicsRenderer renderer;

        public static GraphicsDevice Device { get { return instance.device; } }
        public static Point BackBufferSize
        {
            get
            {
                return new Point(
                    instance.device.PresentationParameters.BackBufferWidth,
                    instance.device.PresentationParameters.BackBufferHeight
                    );
            }
        }
        public static Point WindowSize
        {
            get
            {
                return new Point(
                   instance.device.Viewport.Width,
                   instance.device.Viewport.Height
                   );
            }
        }

        public static GraphicsResourceStorage Cache { get { return instance.resourceCache; } }
        public static GraphicsRenderer Renderer { get { return instance.renderer; } }


        private GraphicsService() { }


        public static void OnResize(int newWidth, int newHeight)
        {
            instance.device.Viewport = new Viewport(0, 0, newWidth, newHeight);
        }

        public static void Initialize(GraphicsDevice device)
        {
            Debug.Assert(instance == null, "GraphicsService is already initialized!");

            instance = new GraphicsService()
            {
                device = device,
            };
            instance.resourceCache = new GraphicsResourceStorage(device);
            instance.renderer = new GraphicsRenderer(device, instance.resourceCache);
        }

        public static void Uninitialize()
        {
            Debug.Assert(instance != null, "GraphicsService is not initialized!");

            instance.renderer.Dispose();
            instance.resourceCache.Dispose();
            instance = null;
        }

        public static void ClearBuffer(Color color)
        {
            instance.device.Clear(color);
        }

        public static void Render()
        {
            Renderer.ExecuteCommands();
        }

    }
}
