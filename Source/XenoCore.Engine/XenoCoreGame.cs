using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Services.Assets;
using XenoCore.Engine.Services.Graphics;
using XenoCore.Engine.Services.Input;

namespace XenoCore.Engine
{
    public class XenoCoreGame : Game 
    {
        private GraphicsDeviceManager graphics;
        private GraphicsService graphicsService;
        private InputService inputService;


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
            ServiceProvider.Add(graphicsService = new GraphicsService(GraphicsDevice, ServiceProvider.Get<AssetsService>()));
            ServiceProvider.Add(inputService =new InputService(new GameTextInput(this)));
        }
        protected override void UnloadContent()
        {
            ServiceProvider.Dispose();
        }

        private StringBuilder sb = new StringBuilder();

        protected override void Update(GameTime gameTime)
        {
            inputService.Update(gameTime);

            inputService.State.UpdateInputText(sb);
               

            //Texture t = graphicsService.ResourceCache.Textures.Get("earth");

            //RenderInstance instance = graphicsService.Renderer.NewTexture(t, 0, BlendingMode.Alpha, 0);

            //instance.Destination = new Rectangle(0, 0, 400, 300);
            //instance.Center = new Vector2(0, 0);
            //instance.TexturePart = new Rectangle(0, 0, 800, 600);
            //instance.Color = Color.White;
            // instance.Rotation = MathHelper.PiOver4;

            //t = graphicsService.ResourceCache.Textures.Get("earth2");
            //instance = graphicsService.Renderer.NewTexture(t, 0, BlendingMode.Alpha, 0);

            //instance.Destination = new Rectangle(100, 100, 400, 300);
            //instance.Center = new Vector2(0, 0);
            //instance.TexturePart = new Rectangle(0, 0, 800, 600);
            //instance.Color = Color.Red;
            //  instance.Rotation = MathHelper.PiOver4;

            Font t = graphicsService.ResourceCache.Fonts.Get("default");

            RenderInstance instance = graphicsService.Renderer.NewFont(t, 0, BlendingMode.Alpha, 0);

            instance.Destination = new Rectangle(0, 0, 0, 0);
            instance.Color = Color.White;
            instance.TextScale = Vector2.One;
            instance.Text = sb.ToString();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphicsService.Renderer.Clear(Color.CornflowerBlue);
            graphicsService.Renderer.ExecuteCommands();

            base.Draw(gameTime);
        }
    }
}
