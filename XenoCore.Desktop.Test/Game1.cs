using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XenoCore.Desktop.Test
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : XenoCore.Engine.XenoCoreGame
    {
        protected override void Initialize()
        {
            GraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            GraphicsDeviceManager.PreferredBackBufferHeight = 720;
            GraphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }
    }
}
