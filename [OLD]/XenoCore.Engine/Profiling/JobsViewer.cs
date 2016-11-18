using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Threading;

namespace XenoCore.Engine.Profiling
{
 
    public class JobsViewer : IProfilerViewer
    {
        public void Update(ProfilerData data) { }

        public void RenderData(ProfilerRenderer renderer, ProfilerData data)
        {
            renderer.DrawRect(new Rectangle(0, 0, renderer.ScreenSize.X, renderer.ScreenSize.Y), Color.Black * 0.5f);

            int boxHeight = 10;
            int fontHeight = 20;
          
            double timeScaleSeconds = 0.0167*3 ;

            int scale = (int)((timeScaleSeconds*5) / 0.0167);

            Vector2 threadSpace = renderer.ScreenSize.ToVector2();
            threadSpace.Y -= fontHeight * 2;
            threadSpace.Y /= data.ThreadsData.Length;
      
            threadSpace.X -= 100;

            double tickToPixels = (threadSpace.X) / (Stopwatch.Frequency * timeScaleSeconds);

            Vector2 offset = new Vector2(20, 20);

            Rectangle rect = new Rectangle();

            rect.Width = 2;
            rect.Y = 0;
            rect.Height = (int)(threadSpace.Y * data.ThreadsData.Length);

            for (int i = 0; i <= scale; ++i)
            {
                Vector2 pos = offset;
                pos.Y += threadSpace.Y * data.ThreadsData.Length;
                pos.X += (i * threadSpace.X) / (scale);
                double timeMS = (timeScaleSeconds * 1000 * i);

                rect.X = (int)pos.X;
                rect.Y = 0;
                rect.Height = (int)pos.Y;// + fontHeight;

                timeMS /= scale;

                Color bgColor = Color.Green;
                if (timeMS >= 16.7)
                    bgColor = Color.Yellow;
                if (timeMS >= 33.4)
                    bgColor = Color.Red;

                if (i < scale)
                    renderer.DrawRect(new Rectangle(rect.X, rect.Y, (int)(threadSpace.X) / (scale), rect.Height), bgColor * 0.25f);

                renderer.DrawRect(rect, Color.DarkGray);
                renderer.DrawText(timeMS.ToString("0.0ms"), pos, Color.White);
            }

            rect.Height = boxHeight;

            long time = data.LastFrame.FrameTime;
            rect.Width = (int)(time * tickToPixels);
            rect.X = (int)(offset.X + data.LastFrame.FrameStart * tickToPixels);
            rect.Y = 10;
            renderer.DrawBox(rect, Color.Blue);

            time = data.LastFrame.DrawTime;
            rect.Width = (int)(time * tickToPixels);
            rect.X = (int)(offset.X + data.LastFrame.DrawStart * tickToPixels);
            rect.Y = 10;
            renderer.DrawBox(rect, Color.LawnGreen);

            time = data.LastFrame.UpdateTime;
            rect.Width = (int)(time * tickToPixels);
            rect.X = (int)(offset.X + data.LastFrame.UpdateStart * tickToPixels);
            rect.Y = 10;
            renderer.DrawBox(rect, Color.Red);

            for (int i = 0; i < data.ThreadsData.Length; ++i)
            {
                var thread = data.ThreadsData[i];

                Vector2 threadOffset = offset + new Vector2(0, threadSpace.Y * i);
                renderer.DrawText(thread.Name, threadOffset, Color.White);

                for (int j = 0; j < thread.Jobs.Count; ++j)
                {
                    long duration = thread.Jobs[j].EndTime - thread.Jobs[j].StartTime;
                    rect.Width = (int)(duration * tickToPixels);
                    rect.X = (int)(threadOffset.X + (thread.Jobs[j].StartTime * tickToPixels));
                    rect.Y = (int)(threadOffset.Y + thread.Jobs[j].Stack * boxHeight + fontHeight);

                    renderer.DrawBox(rect, Color.Red);
                }
            }
        }

    
    }
}
