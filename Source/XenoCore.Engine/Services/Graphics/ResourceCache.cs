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
    public class ResourceStorage<TId, TAsset> where TAsset : class where TId : struct
    {
        private AssetsService assetsService;

        private Dictionary<String, TId> ids = new Dictionary<string, TId>();
        private Dictionary<TId, TAsset> resources = new Dictionary<TId, TAsset>();
        private int idCounter = 0;

        public ResourceStorage(AssetsService assets)
        {
            this.assetsService = assets;
        }

        public TId Get(String resourceName)
        {
            TId id;

            if (!ids.TryGetValue(resourceName, out id))
            {
                TAsset asset = assetsService.Load<TAsset>(resourceName);
                id = Add(asset, resourceName);
            }

            return id;
        }
        public TAsset GetResource(TId id)
        {
            TAsset resource;
            Debug.AssertDebug(resources.TryGetValue(id, out resource), "Invalid resource id!");
            return resource;
        }

        public TId Add(TAsset resource, String resourceName)
        {
            TId id = (TId)Activator.CreateInstance(typeof(TId),++idCounter);
            ids.Add(resourceName, id);
            resources.Add(id, resource);

            return id;
        }

        public void Clear()
        {
            idCounter = 0;
            resources.Clear();
            ids.Clear();
        }
    }


    public class ResourceCache : IDisposable
    {
        private AssetsService assets;
        private Texture2D white;

        public ResourceStorage<Texture, Texture2D> Textures { get; private set; }

        public const String White = "white";

        internal ResourceCache(GraphicsDevice device, AssetsService assets)
        {
            this.assets = assets;

            Textures = new ResourceStorage<Texture, Texture2D>(assets);

            white = new Texture2D(device, 1, 1);
            white.SetData(new Color[] { Color.White });
            white.Name = White;

            Textures.Add(white, white.Name);
        }

        public Texture2D this[Texture id]
        {
            get { return Textures.GetResource(id); }
        }

        public void Dispose()
        {
            Textures.Clear();

            white.Dispose();
        }
    }
}
