using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Threading;
using XenoCore.Engine;
using XenoCore.Engine.Logging;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Profiling;
using XenoCore.Engine.Input;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Console;
using System.Reflection;

namespace XenoCore.Engine.Game
{
    public class XenoCoreGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public ScreenService ScreenService { get; private set; }
        public GameProfiler Profiler { get; private set; }


        public ILoggerProvider LoggerProvider { get; private set; }
        public IThreadProvider threadProvider { get; private set; }
        public ITextInputProvider TextInputProvider { get; private set; }

        private Stack<IDisposable> disposableContainter = new Stack<IDisposable>();

        public Type StartupScreenType { get; set; } = typeof(TestScreen);

        public String ContentPath { get; set; } = "Content";

        [ConsoleVariable(Name = "fps_locked")]
        public bool FrameRateLocked
        {
            get
            {
                return IsFixedTimeStep;
            }
            set
            {
                IsFixedTimeStep = value;
                if (graphics.SynchronizeWithVerticalRetrace !=IsFixedTimeStep)
                {
                    graphics.SynchronizeWithVerticalRetrace = false;
                    graphics.ApplyChanges();  
                }
            }
        }


        public XenoCoreGame(ILoggerProvider loggerProvider, IThreadProvider threadProvider, ITextInputProvider textInputProvider)
        {
            this.LoggerProvider = loggerProvider ?? new DummyLoggerProvider();
            this.threadProvider = threadProvider ?? new DummyThreadProvider();
            this.TextInputProvider = textInputProvider ?? new DummyTextInputProvider();

            graphics = new GraphicsDeviceManager(this);

            //Content = null;
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        //   graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();

         //   IsFixedTimeStep = false;
            IsMouseVisible = true;

        }

        protected override void LoadContent()
        {
            Engine.Services.Initialize(this);
           

            ConsoleService.LoadFromObject(this);

            ScreenService = new ScreenService();
            var screen = Activator.CreateInstance(StartupScreenType) as Screen;

            if (screen == null)
                throw new Exception("StartupScreen value is not a Screen!");

            ScreenService.PushScreen(screen);

            disposableContainter.Push(Profiler = new GameProfiler());
            disposableContainter.Push(new GraphicsDeviceManagerCommands(graphics));
            disposableContainter.Push(new XenoCoreGameCommands(this));
        }
        protected override void UnloadContent()
        {
            while (disposableContainter.Count > 0)
                disposableContainter.Pop().Dispose();

            ConsoleService.UnloadFromObject(this);

            Engine.Services.Uninitialize();
        }

        private bool newFrame = true;

        protected override void Update(GameTime gameTime)
        {
            if(newFrame)
            Profiler.BeginUpdateProfiling(gameTime);

            JobService.Reset();
            InputService.Update(gameTime);

            ScreenService.InputEnabled = !ConsoleService.IsVisible;

            ScreenService.Update(gameTime);

        

            newFrame = false;
        }

        protected override bool BeginDraw()
        {
            return base.BeginDraw();
        }

        protected override void EndDraw()
        {
            base.EndDraw();
        }

        protected override void Draw(GameTime gameTime)
        {
            Profiler.EndUpdateProfiling();

            Profiler.BeginDrawProfiling();

            ScreenService.Draw(gameTime);

            GraphicsService.ClearBuffer(Color.CornflowerBlue);
            GraphicsService.Render();

            Profiler.EndDrawProfiling();

            Profiler.Render(gameTime);

            ConsoleService.Render(gameTime);
            newFrame = true;


        }
    }
}
