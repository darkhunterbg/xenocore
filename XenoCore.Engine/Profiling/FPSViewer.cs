using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Profiling
{


    public class FPSViewer : IProfilerViewer
    {
        public int PlotPointsCount { get; set; } = 300;
        public int MaxFrameRate { get; set; } = 100;

        private List<FrameData> displayFrames = new List<FrameData>();


        public void Update(ProfilerData data)
        {
            displayFrames.Add(data.LastFrame);
            if (displayFrames.Count > PlotPointsCount)
                displayFrames.RemoveAt(0);
        }

        public void RenderData(ProfilerRenderer renderer, ProfilerData data)
        {
        
            renderer.DrawRect(new Rectangle(0, 0, renderer.ScreenSize.X, renderer.ScreenSize.Y), Color.Black * 0.5f);

            Point plotOffset = new Point(40, 0);
            Point plotSize = new Point(renderer.ScreenSize.X - plotOffset.X, renderer.ScreenSize.Y - 40);

            int plotSpace = renderer.ScreenSize.X / (PlotPointsCount - 1);
            int plotFramesCount = MaxFrameRate;

            float frameOffset = (float)plotSize.Y / (float)plotFramesCount;

            renderer.DrawRect(new Rectangle(plotOffset.X - 5, plotOffset.Y, 2, plotSize.Y), Color.DarkGray);

            int linesCount = plotFramesCount / 10;

            for (int i = 0; i < linesCount; ++i)
            {
                int step = plotSize.Y / linesCount;

                Color color = Color.DarkGray;

                Point pos = new Point(plotOffset.X - 5, plotOffset.Y + plotSize.Y - i * step);


                Color bgColor = Color.Green;
                if (i < 6)
                    bgColor = Color.Yellow;
                if (i < 3)
                    bgColor = Color.Red;

                renderer.DrawRect(new Rectangle(pos.X, pos.Y - step, plotSize.X, step), bgColor * 0.25f);

                renderer.DrawRect(new Rectangle(pos.X, pos.Y, plotSize.X, 2), Color.DarkGray);
                renderer.DrawText((i * 10).ToString(), pos.ToVector2() - new Vector2(30, 10), Color.White);
            }

            for (int i = 0; i < displayFrames.Count; ++i)
            {
                Point offset = new Point(plotOffset.X + i * plotSpace, plotOffset.Y + plotSize.Y);

                Point fpsPos = new Point((int)offset.X, (int)offset.Y - (int)(displayFrames[i].FPS * frameOffset));
                double updatePart = displayFrames[i].UpdateEnd / (double)displayFrames[i].FrameTime;
                Point updatePos = new Point((int)offset.X, (int)offset.Y - (int)(displayFrames[i].FPS * frameOffset * updatePart));

                double drawPart = displayFrames[i].DrawEnd / (double)displayFrames[i].FrameTime;
                Point drawPos = new Point((int)offset.X, (int)offset.Y - (int)(displayFrames[i].FPS * frameOffset * drawPart));


                if (displayFrames[i].FrameIndex % 60 == 0)
                {
                    renderer.DrawRect(new Rectangle(fpsPos.X, plotOffset.Y, 2, plotOffset.Y + plotSize.Y), Color.DarkGray);
                    renderer.DrawText(displayFrames[i].GameTime.ToString(), new Vector2(fpsPos.X, plotOffset.Y + plotSize.Y + 5), Color.White);
                }

                if (i + 1 < displayFrames.Count)
                {
                    double nextDrawPart = displayFrames[i + 1].DrawEnd / (double)displayFrames[i + 1].FrameTime;
                    Point nextDrawPos = new Point((int)offset.X + plotSpace, (int)offset.Y - (int)(displayFrames[i + 1].FPS * frameOffset * nextDrawPart));
                    renderer.DrawLine(drawPos - new Point(2, 2), nextDrawPos - new Point(2, 2), 4, Color.GreenYellow);

                    double nextUpdatePart = displayFrames[i+ 1].UpdateEnd / (double)displayFrames[i + 1].FrameTime;
                    Point nextUpdatePos = new Point((int)offset.X + plotSpace, (int)offset.Y -(int)(displayFrames[i + 1].FPS * frameOffset *  nextUpdatePart));
                    renderer.DrawLine(updatePos - new Point(2, 2), nextUpdatePos - new Point(2, 2), 4, Color.Red);

                    Point nextPos = new Point(offset.X + plotSpace, offset.Y - (int)(displayFrames[i + 1].FPS * frameOffset));
                    renderer.DrawLine(fpsPos - new Point(2, 2), nextPos - new Point(2, 2), 4, Color.Blue);
                }


                // renderer.DrawBox(new Rectangle(pos.X - 5, pos.Y - 5, 10, 10), Color.Red);
                //  renderer.DrawText(fps.ToString(), pos.ToVector2() - new Vector2(0, 10), Color.White);
            }

        }
    }
}
