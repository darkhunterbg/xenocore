using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine
{
    public struct UpdateState
    {
        public float DeltaT;
        public bool Paused;
    }
    public struct DrawState
    {
        public float DeltaT;
        public bool Paused;
    }

    public interface IUpdateableSystem
    {
        int UpdateOrder { get; }

        void Update(UpdateState state);
    }

    public interface IDrawableSystem
    {
        int DrawOrder { get; }

        void Draw(DrawState state);
    }


    public static class UpdatingOrder
    {
        public const int PHYSICS = 0;
        public const int ANIMATIONS = 100;
        public const int TIMERS = 200;
        public const int EVENTS = 300;
        public const int SCRIPTS = 400;
    }

    public static class DrawingOrder
    {
        public const int CAMERA = 0;
        public const int WORLD = 100;
        public const int RENDERER = 200;
    }

    public class SystemProvider : IDisposable
    {
        private Dictionary<Type, Object> systems = new Dictionary<Type, object>();
        private Stack<IDisposable> disposableSystems = new Stack<IDisposable>();
        private List<IUpdateableSystem> updateableSystems = new List<IUpdateableSystem>();
        private List<IDrawableSystem> drawableSystems = new List<IDrawableSystem>();

        public void Add<T>(T system) where T : class
        {
            systems.Add(typeof(T), system);
            if (system is IDisposable)
                disposableSystems.Push(system as IDisposable);

            if (system is IUpdateableSystem)
            {
                updateableSystems.Add(system as IUpdateableSystem);
                updateableSystems = updateableSystems.OrderBy(p => p.UpdateOrder).ToList();
            }
            if (system is IDrawableSystem)
            {
                drawableSystems.Add(system as IDrawableSystem);
                drawableSystems = drawableSystems.OrderBy(p => p.DrawOrder).ToList();
            }
        }
        public T Get<T>() where T : class
        {
            Object system = null;
            if (systems.TryGetValue(typeof(T), out system))
                return system as T;

            return null;

        }

        public void Update(GameTime gameTime, bool paused)
        {
            UpdateState state = new UpdateState()
            {
                DeltaT = (float)gameTime.ElapsedGameTime.TotalSeconds,
                Paused = paused,
            };

            foreach (var system in updateableSystems)
                system.Update(state);
        }


        public void Draw(GameTime gameTime, bool paused)
        {
            DrawState state = new DrawState()
            {
                DeltaT = (float)gameTime.ElapsedGameTime.TotalSeconds,
                Paused = paused,
            };

            foreach (var system in drawableSystems)
                system.Draw(state);
        }


        public void Dispose()
        {
            while (disposableSystems.Count > 0)
                disposableSystems.Pop().Dispose();

            systems.Clear();
        }
    }
}
