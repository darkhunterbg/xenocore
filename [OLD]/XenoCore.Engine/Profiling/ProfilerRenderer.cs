using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Resources;

namespace XenoCore.Engine.Profiling
{
    public class ProfilerRenderer
    {
        private GraphicsDevice device;

        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont Font { get; set; }
        public Point ScreenSize
        {
            get
            {
                return new Point(device.Viewport.Width, device.Viewport.Height);
            }
        }

        public ProfilerRenderer(GraphicsDevice device)
        {
            this.device = device;
            SpriteBatch = new SpriteBatch(device);
        }

        public void DrawText(String text, Vector2 position, Color color)
        {
            SpriteBatch.DrawString(Font, text, position, color);
        }
        public void DrawBox(Rectangle rect, Color color)
        {
            var white = GraphicsService.Cache[GraphicsService.Cache.WhiteTexture];

            SpriteBatch.Draw(white, rect, Color.Black * ((float)color.A / 255.0f));
            rect.X += 1;
            rect.Y += 1;
            rect.Width -= 2;
            rect.Height -= 2;
            SpriteBatch.Draw(white, rect, color);
        }

        public void DrawLine(Point start, Point end, int thickness, Color color)
        {
            var white = GraphicsService.Cache[GraphicsService.Cache.WhiteTexture];

            double angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            // angle = MathHelper.Pi;
          //  angle = x;
            int length = (int)(end.ToVector2() - start.ToVector2()).Length();
            SpriteBatch.Draw(white, new Rectangle(start.X, start.Y, length, thickness), null, color, (float)angle, Vector2.Zero, SpriteEffects.None, 1);
            //SpriteBatch.Draw(White, new Rectangle(start.X, start.Y+1, length, thickness -2 ), null, color, (float)angle, Vector2.Zero, SpriteEffects.None, 1);
        }
        public void DrawRect(Rectangle rect, Color color)
        {
            var white = GraphicsService.Cache[GraphicsService.Cache.WhiteTexture];
            SpriteBatch.Draw(white, rect, color);
        }
    }
}
