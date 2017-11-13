using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Editor.Services
{
    public class GraphicsDeviceService : IGraphicsDeviceService
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        // Not used:
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public GraphicsDeviceService(IntPtr handler)
        {
            GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, new PresentationParameters()
            {
                //BackBufferWidth = Math.Max(ClientSize.Width, 1),
                //BackBufferHeight = Math.Max(ClientSize.Height, 1),
                DeviceWindowHandle = handler,
            });
        }
    }
}
