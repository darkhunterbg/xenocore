using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Resources
{

    internal class DummyGraphicsDeviceManager : IGraphicsDeviceService
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        // Not used:
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        public DummyGraphicsDeviceManager(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
    }

    class DummyServiceProvider : IServiceProvider
    {

        public DummyServiceProvider(DummyGraphicsDeviceManager manager)
        {
            Services.Add(typeof(IGraphicsDeviceService), manager);
        }

        public Dictionary<Type, Object> Services { get; private set; } = new Dictionary<Type, object>();
        public object GetService(Type serviceType)
        {
            return Services[serviceType];
        }

    }
}
