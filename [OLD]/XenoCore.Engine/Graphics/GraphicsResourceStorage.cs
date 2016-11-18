using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Resources;

namespace XenoCore.Engine.Graphics
{
    public class GraphicsResourceStorage
    {
        private GraphicsDevice device;

        private int lastid = 0;

        private Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();
        private Dictionary<int, SpriteFont> spriteFonts = new Dictionary<int, SpriteFont>();
        private Dictionary<String, int> resources = new Dictionary<String, int>();

        private Texture2D white;

        //public static Font DefaultFont
        //{
        //    get; private set;
        //}
        public Texture WhiteTexture
        {
            get; private set;
        }


        public Texture2D this[Texture id]
        {
            get { return GetRawTexture(id); }
        }
        public SpriteFont this[Font id]
        {
            get { return GetRawFont(id); }
        }

        internal GraphicsResourceStorage(GraphicsDevice device)
        {
            this.device = device;

            white = new Texture2D(GraphicsService.Device, 1, 1);
            white.Name = "White";

            white.SetData(new Color[] { Color.White });
            WhiteTexture = AddRawTexture(white, "White");
        }
        public void Dispose()
        {
            textures.Clear();
            spriteFonts.Clear();
        }

        public Texture2D GetRawTexture(Texture texture)
        {
            return textures[texture.id];
        }
        public void RemoveTexture(String path)
        {
            var r = resources[path];
            resources.Remove(path);
            textures.Remove(r);
        }
        public void RemoveTexture(Texture texture)
        {
            var r = resources.First(p => p.Value == texture.id);
            resources.Remove(r.Key);
            textures.Remove(texture.id);
        }
        public Texture GetTexture(String path)
        {
            if (String.IsNullOrEmpty(path))
                return WhiteTexture;
            Texture t;
            if (!resources.TryGetValue(path, out t.id))
            {
                var resource = ResourcesService.LoadTexture(path);
                t = AddRawTexture(resource, path);
            }

            return t;
        }
        public Texture AddRawTexture(Texture2D texture, String path)
        {
            int id = 0;

            Debug.Assert(!textures.ContainsValue(texture), "Texture is already added!");

            id = ++lastid;

            textures.Add(id, texture);
            resources.Add(path, id);

            return new Texture()
            {
                id = id,
            };
        }
        public void Flush()
        {
            textures.Clear();
            spriteFonts.Clear();
            resources.Clear();
            lastid = 0;
            AddRawTexture(white, "White");
        }

        public void RemoveFont(String path)
        {
            var r = resources[path];
            resources.Remove(path);
            spriteFonts.Remove(r);
        }
        public SpriteFont GetRawFont(Font font)
        {
            return spriteFonts[font.id];
        }
        public void RemoveFont(Font font)
        {
            var r = resources.First(p => p.Value == font.id);
            resources.Remove(r.Key);
            spriteFonts.Remove(font.id);
        }
        public Font GetFont(String path)
        {
            Font t;
            if (!resources.TryGetValue(path, out t.id))
            {
                var resource = ResourcesService.LoadSpriteFont(path);
                t = AddRawFont(resource, path);
            }

            return t;
        }
        public Font AddRawFont(SpriteFont font, String path)
        {
            int id = 0;

            Debug.Assert(!spriteFonts.ContainsValue(font), "SpriteFont is already added!");

            id = ++lastid;

            spriteFonts.Add(id, font);
            resources.Add(path, id);

            return new Font()
            {
                id = id,
            };
        }
    }
}
