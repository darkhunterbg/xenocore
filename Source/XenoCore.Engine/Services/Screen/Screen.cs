using Microsoft.Xna.Framework;
using System;

namespace XenoCore.Engine.Services.Screen
{
    public class Screen : IDisposable
    {
        public SystemProvider Systems { get; private set; } = new SystemProvider();

        public virtual void UpdateInput(GameTime gameTime)
        {
             
        }
        public virtual void Update(GameTime gameTime, bool paused)
        {
            Systems.Update(gameTime, paused);
        }

        public virtual void Draw(GameTime gameTime, bool paused)
        {
            Systems.Draw(gameTime, paused);
        }

        public virtual void Dispose()
        {
            Systems.Dispose();
        }
    }
}
