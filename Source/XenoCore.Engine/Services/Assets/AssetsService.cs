using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Services.Assets
{
    public class AssetsService
    {
        private ContentManager content;

        public String Root { get { return content.RootDirectory; } set { content.RootDirectory = value; } }

        public AssetsService(ContentManager content)
        {
            this.content = content;
        }

        public T Load<T>(String assetName) where T : class
        {
            return content.Load<T>(assetName);
        }
    }
}
