using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.GUI
{
    public class Button : GUIControl
    {
        public Action Clicked { get; set; }

        private Color originalColor = Color.DarkGray;

        public String Text
        {
            get { return label.Text; }
            set { label.Text = value; }
        }


        private Label label = new Label()
        {
            TextHorizontalAlignment = HorizontalAlignment.Center,
            TextVerticalAlignment = VerticalAlignment.Center,
            Font = Graphics.GraphicsService.Cache.GetFont("default")
        };
        private ImageBox image = new ImageBox()
        {
            Image = Graphics.GraphicsService.Cache.GetTexture("White"),
     
        };

        private AbsoluteContainer container = new AbsoluteContainer();

        public Button()
        {
            image.Color = originalColor;
            container.Children.Add(image);
            container.Children.Add(label);
        }

        public override void Update(GUISystemState systemState)
        {
            if (State.RetrieveEvent(GUIEventType.Click) != null && Clicked != null)
            {
                systemState.EnqueueAction(Clicked);
                container.Margin = new MarginDef(0);
            }

            if (State.RetrieveEvent(GUIEventType.MouseHoverStart) != null)
            {
                image.Color = Color.LightGray;
            }
            if (State.RetrieveEvent(GUIEventType.MouseHoverEnd) != null)
            {
                image.Color = originalColor;
            }
            if (State.RetrieveEvent(GUIEventType.ClickStart) != null)
            {
                int offset = Math.Max(1, Math.Min((int)(State.Bounds.Width * 0.05f), (int)(State.Bounds.Height * 0.05f)));
                container.Margin = new MarginDef(offset);

            }

            container.UpdateState(ref State.Bounds);
            systemState.AddUpdateControl(this, container, false);



        }


    }
}
