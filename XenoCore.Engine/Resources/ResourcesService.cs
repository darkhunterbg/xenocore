using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;

namespace XenoCore.Engine.Resources
{
    public class ResourcesService
    {
        private static ResourcesService instance = null;

        private XenoCoreContentManager content;

        private ResourcesService() { }

        public static String Root
        {
            get { return instance.content.RootDirectory; }

        }

        public static void Initialize(String contentPath, IServiceProvider sp = null)
        {
            Debug.Assert(instance == null, "ResourceService is already initialized!");

            instance = new ResourcesService()
            {
                content = new XenoCoreContentManager(sp, contentPath)
            };
        }
        public static void Uninitialize()
        {
            Debug.Assert(instance != null, "ResourceService is not initialized!");

            instance = null;
        }

        public static void Unload(String path)
        {
            instance.content.Unload(path);
        }
        public static void Reload<T>(String path) where T: class
        {
            instance.content.Reload<T>(path);
        }
        public static T Load<T>(String path) where T : class
        {
            return instance.content.Load<T>(path);
        }

        public static Texture2D LoadTexture(String path)
        {
            return Load<Texture2D>(path);
        }
        public static SpriteFont LoadSpriteFont(String path)
        {
            return Load<SpriteFont>(path);
        }
        public static ParticleEffectDescription LoadParticleEffect(String path)
        {
            return Load<ParticleEffectDescription>(path);
        }
    }
}
