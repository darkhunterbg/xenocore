using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XenoCore.Builder.Host
{
    public enum TextureRenderStrech
    {
        None,
        Fill,
        AspectFit,
    }

    public class TextureRenderControl : GraphicsDeviceControl
    {
        public Texture2D Texture { get; set; }
        public TextureRenderStrech StretchMode { get; set; }

        private SpriteBatch spriteBatch;

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Draw()
        {
            if (Texture == null)
                return;

            GraphicsDevice.Clear(Color.Transparent);

            Rectangle rect = new Rectangle(0, 0, Texture.Width, Texture.Height);

            switch (StretchMode)
            {
                case TextureRenderStrech.Fill:
                    {
                        rect.Width = GraphicsDevice.Viewport.Width;
                        rect.Height = GraphicsDevice.Viewport.Height;
                    }
                    break;
                case TextureRenderStrech.AspectFit:
                    {
                        double p = 1;
                        if (rect.Width > rect.Height)
                        {
                            p = (double)GraphicsDevice.Viewport.Width / (double)rect.Width;

                        }
                        else
                        {
                            p = (double)GraphicsDevice.Viewport.Height / (double)rect.Height;
                        }

                        rect.Width = (int)(rect.Width * p);
                        rect.Height = (int)(rect.Height * p);

                        rect.X = (GraphicsDevice.Viewport.Width - rect.Width) / 2;
                        rect.Y = (GraphicsDevice.Viewport.Height - rect.Height) / 2;
                    }
                    break;
            }

            spriteBatch.Begin();
            spriteBatch.Draw(Texture, rect, Color.White);
            spriteBatch.End();
        }
    }
}
