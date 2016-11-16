using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Threading;
using XenoCore.Engine.Input;
using XenoCore.Engine.Profiling;
using System.Diagnostics;
using XenoCore.Engine.World;
using XenoCore.Engine;

namespace XenoCore.Builder.Host
{
    public partial class ScreenHostControl : GraphicsDeviceControl
    {
        public XenoCore.Engine.Screens.Screen Screen { get; set; }

        private Stopwatch watch = new Stopwatch();


        GameProfiler profiler;

        protected override void Initialize()
        {
            //  profiler = new GameProfiler();
            //  Screen = new TestScreen();
        }

        protected override void Draw()
        {
            if (!Engine.Services.Initialized || Screen == null )
                return;

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            var time = watch.Elapsed;
            watch.Restart();
            var gameTime = new GameTime(time, time);

            JobService.Reset();
            InputService.Update(gameTime);

            Screen.UpdateInput(gameTime);
            Screen.Update(gameTime);

            //  profiler.EndUpdateProfiling();

            //  profiler.BeginDrawProfiling();
            Screen.Draw(gameTime);
            //  profiler.EndDrawProfiling();

            //   profiler.Render(gameTime);

            GraphicsService.Render();

        }
    }
}
