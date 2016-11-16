using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Assets;
using XenoCore.Engine.Services.Graphics;

namespace XenoCore.Engine
{
    public class XenoCoreGame : Game
    {
        private GraphicsDeviceManager graphics;

        public XenoCoreGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            IsMouseVisible = true;

        }

        protected override void LoadContent()
        {
            ServiceProvider.Add(new AssetsService(Content));
            ServiceProvider.Add(new GraphicsService(GraphicsDevice, ServiceProvider.Get<AssetsService>()));

        }
        protected override void UnloadContent()
        {

            ServiceProvider.Dispose();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
