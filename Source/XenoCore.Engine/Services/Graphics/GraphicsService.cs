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
        public GraphicsResourceCache ResourceCache { get; private set; }

        private GraphicsDevice device;

        public GraphicsService(GraphicsDevice device, AssetsService assets)
        {
            this.device = device;

            ResourceCache = new GraphicsResourceCache(device, assets);
        }
    }
}
