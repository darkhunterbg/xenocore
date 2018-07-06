using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace XenoCore.Builder.Host
{

    public class ScreenManagerService
    {
        public List<ScreenHostControl> UpdateControls { get; set; } = new List<ScreenHostControl>();
        private DispatcherTimer timer;


        private Stopwatch watch = new Stopwatch();

        public ScreenManagerService()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000.0 / 120.0);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            foreach (var control in UpdateControls)
            {
                control.Invalidate();
               // control.Invoke(new Action(() => control.Invalidate()));
            }

            watch.Restart();
        }


        public void Start()
        {
            watch.Restart();
            timer.Start();
        }

        public void Stop()
        {
            watch.Stop();
            timer.Stop();
        }
    }
}
