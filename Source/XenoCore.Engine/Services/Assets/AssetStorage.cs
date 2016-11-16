using System;
using System.Collections.Generic;

namespace XenoCore.Engine.Services.Assets
{
    public class AssetStorage<TId, TAsset> where TAsset : class where TId : struct
    {
        private AssetsService assetsService;

        private Dictionary<String, TId> ids = new Dictionary<string, TId>();
        private Dictionary<TId, TAsset> resources = new Dictionary<TId, TAsset>();
        private int idCounter = 0;

        public AssetStorage(AssetsService assets)
        {
            this.assetsService = assets;
        }

        public TId Get(String assetName )
        {
            TId id;

            if (!ids.TryGetValue(assetName, out id))
            {
                TAsset asset = assetsService.Load<TAsset>(assetName);
                id = Add(asset, assetName);
            }

            return id;
        }
        public TAsset GetResource(TId id)
        {
            TAsset resource;
            Debug.AssertDebug(resources.TryGetValue(id, out resource), "Invalid resource id!");
            return resource;
        }

        public TId Add(TAsset asset, String asssetName)
        {
            TId id = (TId)Activator.CreateInstance(typeof(TId), ++idCounter);
            ids.Add(asssetName, id);
            resources.Add(id, asset);

            return id;
        }

        public void Clear()
        {
            idCounter = 0;
            resources.Clear();
            ids.Clear();
        }
    }
}
