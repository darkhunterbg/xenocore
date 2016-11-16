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
    public class ResourceCache : IDisposable
    {
        private AssetsService assets;
        private Texture2D white;

        public AssetStorage<Texture, Texture2D> Textures { get; private set; }
        public AssetStorage<Font,SpriteFont> Fonts { get; private set; }

        public const String White = "white";
        public const String Default = "default";

        internal ResourceCache(GraphicsDevice device, AssetsService assets)
        {
            this.assets = assets;

            Textures = new AssetStorage<Texture, Texture2D>(assets);
            Fonts = new AssetStorage<Font, SpriteFont>(assets);

            white = new Texture2D(device, 1, 1);
            white.SetData(new Color[] { Color.White });
            white.Name = White;

            Textures.Add(white, white.Name);
        }

        public Texture2D this[Texture id]
        {
            get { return Textures.GetResource(id); }
        }
        public SpriteFont this[Font id]
        {
            get { return Fonts.GetResource(id); }
        }

        public void Dispose()
        {
            Textures.Clear();
            Fonts.Clear();

            white.Dispose();
        }
    }
}
