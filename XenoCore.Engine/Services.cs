using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Game;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Input;
using XenoCore.Engine.Logging;
using XenoCore.Engine.Resources;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Threading;

namespace XenoCore.Engine
{
    public static class Services
    {
        public static bool Initialized { get; private set; } = false;



        [ConsoleCommand(Name = "version")]
        public static void Version()
        {
            var assembly = typeof(Services).GetTypeInfo().Assembly;
            var name = new AssemblyName(assembly.FullName);
            ConsoleService.Msg($"XenoCore Engine {name.Version.Major}.{name.Version.Minor} build {name.Version.Build}");
        }

        public static void Initialize(XenoCoreGame game)
        {
            Initialize(game.ContentPath, game.Services, game.LoggerProvider, game.threadProvider, game.TextInputProvider);
        }


        //TODO : FIX THIS

        public static void Initialize(String contentPath, IServiceProvider serviceProvider, // , GraphicsDevice device,
            ILoggerProvider loggerProvider, IThreadProvider threadProvider, ITextInputProvider textProvider)
        {
            Debug.Assert(!Initialized, "Services are already initialized!");

            var device = (serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService).GraphicsDevice;

            LoggingService.Initialize(loggerProvider);
            GraphicsService.Initialize(device);
            ResourcesService.Initialize(contentPath, serviceProvider);
            ConsoleService.Initialize();
            JobService.Initialize(threadProvider);
            InputService.Initialize(textProvider);

            Initialized = true;
        }

        public static void Uninitialize()
        {
            Debug.Assert(Initialized, "Services were not initialized!");

            InputService.Uninitialize();
            JobService.Uninitialize();
            ConsoleService.Uninitialize();
            ResourcesService.Uninitialize();
            GraphicsService.Uninitialize();
            LoggingService.Uninitialize();

            Initialized = false;
        }
    }
}
