using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Input;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Threading;
using XenoCore.Engine.World;

namespace XenoCore.Engine.GUI
{
    public class GUISystem : IDrawableSystem, IDisposable
    {
        private GUIControl root;

        public GUIControl RootControl
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
                if (root != null)
                    root.ShouldUpdate = true;
            }
        }

        [ConsoleVariable(Name = "gui_debug")]
        public bool Debug { get; set; } = false;

        public const int DRAW_ORDER = DrawingOrder.GUI;
        public int DrawOrder { get { return DRAW_ORDER; } }

        private GUISystemState state;
        private List<GUIControl> updateControls = new List<GUIControl>();

        internal ConcurrentQueue<Action> ControlActionsQueue { get; private set; } = new ConcurrentQueue<Action>();

        public List<GUIEvent> QueuedEvents { get; private set; } = new List<GUIEvent>();

        public GUISystem()
        {
            state = new GUISystemState(this);
            ConsoleService.LoadFromObject(this);
        }

        public void UpdateInput()
        {

            if (RootControl == null)
                return;

            var pos = InputService.InputState.CurrentInput.Mouse.Position;

            if (InputService.InputState.WasMouseButtonPressed(MouseButton.Left))
            {

                QueuedEvents.Add(new GUIMouseClickEvent(true)
                {
                    Button = MouseButton.Left,
                    Position = pos,
                });
            }

            if (InputService.InputState.WasMouseButtonReleased(MouseButton.Left))
            {

                QueuedEvents.Add(new GUIMouseClickEvent()
                {
                    Button = MouseButton.Left,
                    Position = pos,
                });
            }

            //TODO : enqueue only if there was some changes (also could be recycled)
            QueuedEvents.Add(new GUIMouseHoverEvent()
            {
                Position = pos
            });
        }

        public void Draw(DrawState drawState)
        {
            state.CameraRenderStateIndex = 1;
            state.DeltaT = drawState.DeltaT;

            if (RootControl != null && RootControl.Visiblity == Visiblity.Visible)
            {
                var screenSize = GraphicsService.BackBufferSize;
                Rectangle rect = new Rectangle(0, 0, screenSize.X, screenSize.Y);

                if (RootControl.ShouldUpdate)
                    RootControl.UpdateState(ref rect);

                UpdateControlEvents(RootControl, QueuedEvents);
                updateControls.Add(RootControl);

                while (updateControls.Count > 0)
                {
                    var task = JobServiceExtender.RunJobsBatched(updateControls.Count, 10, UpdateControlsJob);
                    JobService.WaitForJobs(task);

                    updateControls.Clear();

                    foreach (var data in state.ThreadData.Data)
                    {
                        updateControls.AddRange(data.UpdateControls);
                        data.UpdateControls.Clear();
                    }
                }
            }

            Action action = null;
            while (ControlActionsQueue.TryDequeue(out action))
            {
                action();
            }

            QueuedEvents.Clear();
        }

        private void UpdateControlsJob(Object param)
        {
            BatchedJobData data = (BatchedJobData)param;

            for (int i = data.StartIndex; i < data.EndIndex; ++i)
            {
                var control = updateControls[i];
                //TODO : request state update
                //TODO : controls tree 

                if (control.ShouldUpdate)
                {
                    control.Update(state);
                  //  control.ShouldUpdate = false;
                }


                control.State.Events.Clear();

                if (Debug)//&& !( control is GUIContainer))
                {
                    var r = control.State.Bounds.ToRectangleF();
                    GraphicsRendererExtender.RenderBox(ref r, Color.Red * 0.25f, 3, 0);
                }
            }
        }

        internal void UpdateControlEvents(GUIControl control, List<GUIEvent> events)
        {
            foreach (var e in events)
            {
                switch (e.Type)
                {
                    case GUIEventType.MouseClick:
                        {
                            var me = e.Cast<GUIMouseClickEvent>();

                            if (control.State.Bounds.Contains(me.Position))
                            {
                                control.State.Events.Add(e);
                                control.State.Events.Add(new GUIEvent(GUIEventType.Click));
                            }
                            break;
                        }
                    case GUIEventType.MouseClickStart:
                        {

                            var me = e.Cast<GUIMouseClickEvent>();

                            if (control.State.Bounds.Contains(me.Position))
                            {
                                control.State.Events.Add(e);
                                control.State.Events.Add(new GUIEvent(GUIEventType.ClickStart));
                            }

                            break;
                        }
                    //case GUIEventType.MouseHoverEnd:
                    //    {
                    //        var me = e.Cast<GUIMouseHoverEvent>();
                    //        if (control.State.MouseHovered)
                    //        {
                    //            control.State.MouseHovered = false;
                    //            control.State.Events.Add(e);
                    //        }
                    //    }
                    //    break;
                    case GUIEventType.MouseHover:
                        {
                            //TODO: Better mouse event handling
                            var me = e.Cast<GUIMouseHoverEvent>();
                            var inside = control.State.Bounds.Contains(me.Position);

                            control.State.Events.Add(e);

                            if (control.State.MouseHovered)
                            {
                                if (!inside)
                                {
                                    //Еntering
                                    control.State.MouseHovered = false;
                                    control.State.Events.Add(new GUIMouseHoverEvent(control.State.MouseHovered)
                                    {
                                        Position = me.Position
                                    });
                                }
                            }
                            else
                            {
                                if (inside)
                                {
                                    //Exiting
                                    control.State.MouseHovered = true;
                                    control.State.Events.Add(new GUIMouseHoverEvent(control.State.MouseHovered)
                                    {
                                        Position = me.Position
                                    });
                                }
                            }



                            break;
                        }

                }
            }

        }

        public void Dispose()
        {
            ConsoleService.UnloadFromObject(this);
        }
    }
}
