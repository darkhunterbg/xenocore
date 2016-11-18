using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Screens
{
    public class ScreenService 
    {
        private List<Screen> screensStack = new List<Screen>();

        public bool InputEnabled { get; set; } = true;

        private Screen CurrentScreen
        {
            get { return screensStack.Count == 0 ? null : screensStack.Last(); }
        }

        public ScreenService()
        {

        }

        public void Initialize()
        {
        }

        public  void Uninitialize()
        {
        }

        public void PushScreen(Screen screen)
        {
            screensStack.Add(screen);
        }

        public void Update(GameTime time)
        {
            if (InputEnabled)
                CurrentScreen?.UpdateInput(time);

            CurrentScreen?.Update(time);
        }
      
        public void Draw(GameTime time)
        {
            CurrentScreen?.Draw(time);
        }
    }
}
