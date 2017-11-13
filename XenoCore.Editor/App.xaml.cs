using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XenoCore.Editor.Services;

namespace XenoCore.Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public AppServiceProvider Services { get; private set; } = new AppServiceProvider();

        public static App CurrentApp { get { return Current as App; } }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Services.AddService<IGraphicsDeviceService>(new GraphicsDeviceService(IntPtr.Zero));
        }

    }
}
