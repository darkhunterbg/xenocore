using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Input;

namespace XenoCore.Engine.Screens
{
    public abstract class Screen : IDisposable
    {
        public SystemProvider Systems { get; private set; } = new SystemProvider();

        [ConsoleVariable(Name = "paused")]
        public bool Paused { get; set; } = false;

        [ConsoleCommand(Name = "step")]
        public void UpdateStep()
        {
            if(!Paused)
            {
                ConsoleService.Warning("Cannot step when paused = false");
                return;
            }

            Systems.Update(new GameTime(new TimeSpan(),TimeSpan.FromSeconds(0.016667)),false);
        }

        public virtual void UpdateInput(GameTime gameTime)
        {
            if (Paused && InputService.InputState.WasKeyReleased(Keys.F3))
                UpdateStep();
        }
        public virtual void Update(GameTime gameTime)
        {
            Systems.Update(gameTime, Paused);
        }

        public virtual void Draw(GameTime gameTime)
        {
            Systems.Draw(gameTime, Paused);
        }

        public virtual void Dispose()
        {
            Systems.Dispose();
        }


    }
}
