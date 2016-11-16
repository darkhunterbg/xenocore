using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;

namespace XenoCore.Engine.Resources
{
    public class XenoCoreContentManager : ContentManager
    {
        private static IServiceProvider serviceProvider;

        private static IServiceProvider DefaultServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                    serviceProvider = new DummyServiceProvider(
                        new DummyGraphicsDeviceManager(GraphicsService.Device));

                return serviceProvider;
            }
        }
        
        public XenoCoreContentManager(IServiceProvider sp, String rootDir) :
            base(sp ?? DefaultServiceProvider, rootDir)
        {
            serviceProvider = sp;
        }

        public XenoCoreContentManager(String rootDir)
            : base(null, rootDir)
        {

        }

        public Object Unload(String path)
        {
            var asset = LoadedAssets[path];
            LoadedAssets.Remove(path);

            if (asset is IDisposable)
                (asset as IDisposable).Dispose();

            return asset;
        }

        public T Reload<T>(String path) where T:class
        {
            var asset = LoadedAssets[path] as T;
            ReloadAsset<T>(path, asset);

            return asset;
        }
    }
}
