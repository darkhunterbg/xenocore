using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Threading;
using System.Collections;

namespace XenoCore.Engine.GUI
{
    public enum HorizontalAlignment
    {
        Left, Right, Center
    }
    public enum VerticalAlignment
    {
        Top, Bottom, Center
    }
    public enum Visiblity
    {
        Visible,
        Hidden,
        Collapsed,
    }


    public struct MarginDef
    {
        public int Left, Top, Right, Bottom;

        public int Width { get { return Left + Right; } }
        public int Height { get { return Top + Bottom; } }

        public MarginDef(int margin)
        {
            Left = Top = Right = Bottom = margin;
        }
        public MarginDef(int left =0, int top = 0, int right = 0, int botton = 0)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = botton;
        }
    }
    public struct Dimension
    {
        public static readonly Dimension Auto = new Dimension(-0xffff);

        private float Value;


        public int Absolute
        {
            get { return (int)(Value > 0 ? Value : 0); }
            set { Value = value > 0 ? value : 0; }
        }
        public float Percentage
        {
            get { return (Value < 0 ? -Value : 0); }
            set
            {
                Value = value > 0 ? -value : 0; ;
            }
        }
        public bool IsAuto
        {
            get { return Value == Auto.Value; }
        }
        public bool IsAbsolute
        {
            get { return Value >= 0; }
        }

        public Dimension(int val)
        {
            this.Value = val;
        }
        public Dimension(float val)
        {
            this.Value = -val;
        }
    }


    public abstract class GUIControl
    {
        public MarginDef Margin;
        public Point Size;
        public HorizontalAlignment HorizontalAlignment;
        public VerticalAlignment VerticalAlignment;
        public float Weight = 0;
        public Visiblity Visiblity = Visiblity.Visible;
        public String Name { get; set; } = String.Empty;
        public Dictionary<String, Object> ControlsData { get; private set; } = new Dictionary<string, object>();
        public GUIControl Parent { get; internal set; }

        public ControlState State { get; private set; } = new ControlState();

        public bool ShouldUpdate { get; set; }


        public virtual void Update(GUISystemState systemState) { }
   
        public void UpdateState(ref Rectangle space)
        {
            this.ShouldUpdate = true;

            State.Bounds = space;

            if (Visiblity == Visiblity.Collapsed)
            {
                State.Bounds = new Rectangle();
                return;
            }

            State.Bounds.X += Margin.Left;
            State.Bounds.Y += Margin.Top;
            State.Bounds.Width -= Margin.Width;
            State.Bounds.Height -= Margin.Height;

            if (Size.X > 0 && Size.X < State.Bounds.Width)
                State.Bounds.Width = Size.X;
            if (Size.Y > 0 && Size.Y < State.Bounds.Height)
                State.Bounds.Height = Size.Y;

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Right:
                    State.Bounds.X += State.Bounds.Width - State.Bounds.Width;
                    break;
                case HorizontalAlignment.Center:
                    State.Bounds.X += (State.Bounds.Width - State.Bounds.Width) / 2;
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    State.Bounds.Y += State.Bounds.Height - State.Bounds.Height;
                    break;
                case VerticalAlignment.Center:
                    State.Bounds.Y += (State.Bounds.Height - State.Bounds.Height) / 2;
                    break;
            }
        }
    }

    public class GUISystemState
    {
        public bool Debug { get { return system.Debug; } }
        public float DeltaT { get; internal set; }
        public int CameraRenderStateIndex { get; set; }

        private GUISystem system;

        public GUISystemState(GUISystem system)
        {
            this.system = system;
        }

        internal PerThreadData<GUIThreadData> ThreadData { get; private set; } = new PerThreadData<GUIThreadData>();

        public void FowardEvents(GUIControl parent, GUIControl control)
        {
            system.UpdateControlEvents(control, parent.State.Events);
   
        }

        public void AddUpdateControl(GUIControl parent, GUIControl control, bool forwardEvents = true)
        {
            if (forwardEvents)
                FowardEvents(parent, control);

                ThreadData.Current.UpdateControls.Add(control);
        }
        public void EnqueueAction(Action action)
        {
            system.ControlActionsQueue.Enqueue(action);
        }

    }

    public abstract class GUIContainer : GUIControl
    {
        public class GUIChildrenCollection : IList<GUIControl>
        {
            private GUIContainer owner;
            private List<GUIControl> collection = new List<GUIControl>();

            internal GUIChildrenCollection(GUIContainer owner) { this.owner = owner; }


            public GUIControl this[int index]
            {
                get
                {
                    return ((IList<GUIControl>)collection)[index];
                }

                set
                {
                    ((IList<GUIControl>)collection)[index] = value;
                }
            }

            public int Count
            {
                get
                {
                    return ((IList<GUIControl>)collection).Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return ((IList<GUIControl>)collection).IsReadOnly;
                }
            }

            public void Add(GUIControl item)
            {
                Debug.Assert(item.Parent == null, "GUIControl already has  aparent!");

                ((IList<GUIControl>)collection).Add(item);
                item.Parent = owner;
            }

            public void Clear()
            {
                foreach (var control in collection)
                    control.Parent = null;

                ((IList<GUIControl>)collection).Clear();
            }

            public bool Contains(GUIControl item)
            {
                return ((IList<GUIControl>)collection).Contains(item);
            }

            public void CopyTo(GUIControl[] array, int arrayIndex)
            {
                ((IList<GUIControl>)collection).CopyTo(array, arrayIndex);
            }

            public IEnumerator<GUIControl> GetEnumerator()
            {
                return ((IList<GUIControl>)collection).GetEnumerator();
            }

            public int IndexOf(GUIControl item)
            {
                return ((IList<GUIControl>)collection).IndexOf(item);
            }

            public void Insert(int index, GUIControl item)
            {
                Debug.Assert(item.Parent == null, "GUIControl already has  aparent!");

                ((IList<GUIControl>)collection).Insert(index, item);
                item.Parent = owner;
            }

            public bool Remove(GUIControl item)
            {
                bool result = ((IList<GUIControl>)collection).Remove(item);

                if (result)
                    item.Parent = null;

                return result;
            }

            public void RemoveAt(int index)
            {
                collection[index].Parent = null;
                ((IList<GUIControl>)collection).RemoveAt(index);

            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IList<GUIControl>)collection).GetEnumerator();
            }
        }


        public IList<GUIControl> Children { get; private set; }

        public GUIContainer()
        {
            Children = new GUIChildrenCollection(this);
        }

        public float GetTotalWeight()
        {
            float result = 0;
            for (int i = 0; i < Children.Count; ++i)
                result += Children[i].Weight;

            return result;
        }
    }
}
