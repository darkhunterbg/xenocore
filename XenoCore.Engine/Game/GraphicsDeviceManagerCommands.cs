using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;

namespace XenoCore.Engine.Game
{
    public class GraphicsDeviceManagerCommands : ConsoleAutoRegister
    {
        private GraphicsDeviceManager manager;

        public GraphicsDeviceManagerCommands(GraphicsDeviceManager manager)
        {
            this.manager = manager;
            Register();
        }

        [ConsoleVariable(Name = "fullscreen")]
        public bool IsFullScreen
        {
            get
            {
                return manager.IsFullScreen;
            }
            set
            {
                if (manager.IsFullScreen != value)
                {
                    manager.IsFullScreen = value;
                    manager.ApplyChanges();
                }
            }
        }
        [ConsoleVariable(Name = "resolution")]
        public String Resolution
        {
            get
            {
                return $"{manager.PreferredBackBufferWidth}x{manager.PreferredBackBufferHeight}";
            }
            set
            {
                var split = value.Split(new char[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    int width = 0, height = 0;
                    if (int.TryParse(split[0], out width) &&
                        int.TryParse(split[1], out height) &&
                        width >= 0 && height >= 0)
                    {
                        manager.PreferredBackBufferWidth = width;
                        manager.PreferredBackBufferHeight = height;
                        manager.ApplyChanges();
                    }
                }
            }
        }
    }
}
