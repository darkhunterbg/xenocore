using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Assets;
using XenoCore.Engine.Services.Audio;
using XenoCore.Engine.Services.Graphics;
using XenoCore.Engine.Services.Input;
using XenoCore.Engine.Services.Screen;

namespace XenoCore.Engine
{
    public class XenoCoreGame : Game
    {
        private GraphicsDeviceManager graphics;
        private GraphicsService graphicsService;
        private InputService inputService;
        private AudioService audioService;
        private ScreenService screenService;

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

            ServiceProvider.Add(new AssetsService(Content));
            ServiceProvider.Add(graphicsService = new GraphicsService(GraphicsDevice, ServiceProvider.Get<AssetsService>()));
            ServiceProvider.Add(inputService = new InputService(new GameTextInput(this)));
            ServiceProvider.Add(audioService = new AudioService(ServiceProvider.Get<AssetsService>()));
            ServiceProvider.Add(screenService = new ScreenService());

            screenService.PushScreen(new TestScreen());

        }

        protected override void LoadContent()
        {


        }
        protected override void UnloadContent()
        {
            ServiceProvider.Dispose();
        }

        private StringBuilder sb = new StringBuilder();

        protected override void Update(GameTime gameTime)
        {
            audioService.Update(gameTime);

            inputService.Update(gameTime);

            screenService.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            screenService.Draw(gameTime);
            graphicsService.Renderer.Clear(Color.CornflowerBlue);
            graphicsService.Renderer.ExecuteCommands();

            base.Draw(gameTime);
        }
    }
}
