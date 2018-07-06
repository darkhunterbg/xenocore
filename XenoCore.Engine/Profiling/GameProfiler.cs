using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Input;
using XenoCore.Engine.Resources;
using XenoCore.Engine.Threading;

namespace XenoCore.Engine.Profiling
{
    public class GameProfiler : IDisposable
    {
        public JobsViewer JobsViewer { get; private set; } = new JobsViewer();
        public FPSViewer FPSViewer { get; private set; } = new FPSViewer();

        private float deltaT = 0;

        private IProfilerViewer[] viewers;

        private ProfilerData Data { get; set; }

        private ProfilerRenderer renderer;

        private GameTime gameTime;
        private Stopwatch frameWatch = new Stopwatch();

        [ConsoleVariable(Name = "profiler")]
        public bool ShowProfiler { get; set; } = false;
        [ConsoleVariable(Name = "fps")]
        public bool ShowFPS { get; set; } = false;

        private int frameIndex = 0;
        private int viewerIndex = 0;

        private bool profilingStarted = false;

        private InputAction showHideAction = new InputAction(state =>
        {
            return state.WasKeyReleased(Keys.F1);
        });
        private InputAction nextAction = new InputAction(state =>
        {
            return state.WasKeyReleased(Keys.F2);
        });


        public GameProfiler()
        {
            viewers = new IProfilerViewer[] { FPSViewer, JobsViewer };

            renderer = new ProfilerRenderer(GraphicsService.Device);
            renderer.Font = GraphicsService.Cache[GraphicsService.Cache.GetFont("default")];

            Data = new ProfilerData()
            {
                ThreadsData = new ThreadPerformanceData[JobService.Threads.Length]
            };

#if DEBUG
            ShowFPS = true;
#endif

            ConsoleService.LoadFromObject(this);


        }

        public void Dispose()
        {
            ConsoleService.UnloadFromObject(this);
        }

        [ConsoleCommand(Name = "prof_prev")]
        public void PreviousViewer()
        {
            --viewerIndex;
            if (viewerIndex < 0)
                viewerIndex = viewers.Length - 1;
        }

        [ConsoleCommand(Name = "prof_next")]
        public void NextViewer()
        {
            ++viewerIndex;
            if (viewerIndex == viewers.Length)
                viewerIndex = 0;
        }


        public void BeginUpdateProfiling(GameTime time)
        {
            this.gameTime = time;
            Data.CollectingFrame.FrameEnd = frameWatch.ElapsedTicks;
            Data.CollectingFrame.FrameTime = Data.CollectingFrame.FrameEnd - Data.CollectingFrame.FrameStart;

            SmoothTime(ref Data.LastFrame, ref Data.CollectingFrame);

            Data.CollectingFrame.FPS = (Stopwatch.Frequency / (double)Data.CollectingFrame.FrameTime);

            Data.LastFrame = Data.CollectingFrame;

            frameWatch.Restart();

            Data.CollectingFrame.FrameStart = frameWatch.ElapsedTicks;
            Data.CollectingFrame.UpdateStart = frameWatch.ElapsedTicks;
            Data.CollectingFrame.FrameIndex = ++frameIndex;
            Data.CollectingFrame.GameTime = time.TotalGameTime;

            profilingStarted = false;

            if (ShowProfiler)
            {
                profilingStarted = true;
                JobService.StartDataCollection();
            }
        }

        public void EndUpdateProfiling()
        {
            Data.CollectingFrame.UpdateEnd = frameWatch.ElapsedTicks;
            Data.CollectingFrame.UpdateTime = Data.CollectingFrame.UpdateEnd - Data.CollectingFrame.UpdateStart;
        }

        public void BeginDrawProfiling()
        {
            Data.CollectingFrame.DrawStart = frameWatch.ElapsedTicks;
        }

        public void EndDrawProfiling()
        {
            Data.CollectingFrame.DrawEnd = frameWatch.ElapsedTicks;
            Data.CollectingFrame.DrawTime = Data.CollectingFrame.DrawEnd - Data.CollectingFrame.DrawStart;

            if (profilingStarted && JobService.IsProfiling)
            {
                JobService.EndDataCollection();

                Data.ThreadsData = new ThreadPerformanceData[JobService.Threads.Length];
                for (int i = 0; i < Data.ThreadsData.Length; ++i)
                {
                    var thread = JobService.Threads[i];
                    Data.ThreadsData[i] = new ThreadPerformanceData();
                    Data.ThreadsData[i].Name = thread.Name;
                    Data.ThreadsData[i].Jobs = thread.JobStats.Take(thread.JobStatsCount).ToList();
                }
            }
        }


        public void Render(GameTime gameTime)
        {
#if DEV
            if (InputService.IsInputActionTriggered(showHideAction))
                ShowProfiler = !ShowProfiler;

            if (ShowProfiler && InputService.IsInputActionTriggered(nextAction))
                NextViewer();

            if (Data.LastFrame.FrameIndex <= 1)
                return;

            foreach (var viewer in viewers)
                viewer.Update(Data);
#endif

            Vector2 scale = (GraphicsService.BackBufferSize.ToVector2() ) / (GraphicsService.WindowSize.ToVector2());

            renderer.SpriteBatch.Begin(SpriteSortMode.BackToFront,null,null,null,null,null, Matrix.CreateScale(new Vector3(scale, 1)));

            if (ShowProfiler)
                viewers[viewerIndex].RenderData(renderer, Data);

            if (ShowFPS)
                renderer.DrawText($"FPS:{Data.LastFrame.FPS:0}", Vector2.Zero, Color.White);

            renderer.SpriteBatch.End();
        }


        private void SmoothTime(ref FrameData prev, ref FrameData current)
        {
            double newCoeff = 0.2f;
            double prevCoeff = 1.0 - newCoeff;

            current.FrameTime = (long)(current.FrameTime * newCoeff + prev.FrameTime * prevCoeff);
            current.DrawTime = (long)(current.DrawTime * newCoeff + prev.DrawTime * prevCoeff);
            current.UpdateTime = (long)(current.UpdateTime * newCoeff + prev.UpdateTime * prevCoeff);
        }
    }
}
