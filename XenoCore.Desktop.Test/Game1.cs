using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XenoCore.Desktop.Test
{
    public class Game1 : XenoCore.Engine.XenoCoreGame
    {
        public Game1()
        {
           
        }

        protected override void Initialize()
        {
            base.Initialize();

            GraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            GraphicsDeviceManager.PreferredBackBufferHeight = 720;
            GraphicsDeviceManager.ApplyChanges();
        }
    }
}
