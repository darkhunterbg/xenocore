using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Assets;

namespace XenoCore.Engine.Services.Graphics
{
    public class GraphicsService
    {
        private GraphicsDevice device;

        public ResourceCache ResourceCache { get; private set; }
        public Renderer Renderer { get; private set; }


        public Point BackBufferSize
        {
            get
            {
                return new Point(
                    device.PresentationParameters.BackBufferWidth,
                    device.PresentationParameters.BackBufferHeight
                    );
            }
        }
        public Point ViewportSize
        {
            get
            {
                return new Point(
                   device.Viewport.Width,
                   device.Viewport.Height
                   );
            }
        }

        public GraphicsService(GraphicsDevice device, AssetsService assets)
        {
            this.device = device;

            ResourceCache = new ResourceCache(device, assets);
            Renderer = new Renderer(device, ResourceCache);
            
        }

    }
}
