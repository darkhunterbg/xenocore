using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
//using XenoCore.Desktop;
//using XenoCore.Engine;
using Microsoft.Xna.Framework.Graphics;
using XenoCore.Engine.Graphics;
using XenoCore.Desktop;
using XenoCore.Engine;
using XenoCore.Builder.Host;
using Microsoft.Xna.Framework.Content;
using XenoCore.Engine.Logging;
using XenoCore.Engine.Input;
using System.IO;
using XenoCore.Builder.Services;
using XenoCore.Engine.Console;
using XenoCore.Builder.GUI;
using XenoCore.ContentPipeline;
using XenoCore.Engine.Resources;

//https://www.reddit.com/r/dotnet/comments/49kewh/xna_graphicsdevice_type_load_exception/

namespace XenoCore.Builder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App CurrentApp
        {
            get { return App.Current as App; }
        }

        public ContentManager Content { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        public ScreenManagerService ScreenManager { get; set; } = new ScreenManagerService();

        //public Lazy<ParticleEffectWindow> ParticleEffectEditor { get; private set; } = new Lazy<ParticleEffectWindow>();
        public MainWindow MainEditor { get; internal set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DesktopPlatform.Initialize();
            ConfigurationService.Initialize();
        }

        public void LoadServices(ContentPipelineProject project)
        {
            if (Engine.Services.Initialized)
                Engine.Services.Uninitialize();

            var serviceProvider = new DummyServiceProvider(new DummyGraphicsDeviceManager(GraphicsDeviceControl.GraphicsDevice));

            XenoCore.Engine.Services.Initialize(project.OutputDir, serviceProvider,
                new DummyLoggerProvider(), DesktopPlatform.ThreadProvider, new DummyTextInputProvider());

            ResourceManagerService.UnloadResources();
            ResourceManagerService.UsePipelineProject(project);

            ConsoleService.DisableObjectsLoading = true;

            ScreenManager.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ScreenManager.Stop();

            if (Engine.Services.Initialized)
                Engine.Services.Uninitialize();
             
            DesktopPlatform.Uninitialize();
            ConfigurationService.Uninitialize();

            base.OnExit(e);
        }
    }
}
