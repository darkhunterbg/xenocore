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

    class UpdaterInstance
    {
        public IUpdateableSystem System;
        public float DeltaT;
        public int Order;
    }


    public interface IUpdateableSystem
    {
        int UpdateOrder { get; }

        //0 means each frame
        uint UpdatesPerSecond { get; }

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
        public const int EVENTS = 100;
        public const int SCRIPTS = 200;

        public const int RENDER = 400;
    }

    public static class DrawingOrder
    {
        public const int GUI = 0;
        public const int WORLD = 100;
    }



    public class SystemProvider : IDisposable
    {
        private Dictionary<Type, Object> systems = new Dictionary<Type, object>();
        private Stack<IDisposable> disposableSystems = new Stack<IDisposable>();
        private List<UpdaterInstance> updateableSystems = new List<UpdaterInstance>();
        private List<IDrawableSystem> drawableSystems = new List<IDrawableSystem>();

        public void Register<T>(T system) where T : class
        {
            systems.Add(typeof(T), system);
            if (system is IDisposable)
                disposableSystems.Push(system as IDisposable);
            if (system is IUpdateableSystem)
            {
                var s = system as IUpdateableSystem;
                UpdaterInstance instance = new UpdaterInstance()
                {
                    DeltaT = 0,
                    Order = s.UpdateOrder,

                    System = s,
                };

                updateableSystems.Add(instance);
                updateableSystems = updateableSystems.OrderBy(p => p.Order).ToList();
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
            if (gameTime.ElapsedGameTime.TotalSeconds > 2.0)
                return;

            float deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateState state = new UpdateState();
            state.Paused = paused;

            foreach (var instance in updateableSystems)
            {
                var frameTime = instance.System.UpdatesPerSecond == 0 ?
                     0 : 1.0f / (float)instance.System.UpdatesPerSecond;

                if (frameTime == 0)
                {
                    state.DeltaT = deltaT;
                    instance.System.Update(state);
                }
                else
                {
                    state.DeltaT = frameTime;
                    instance.DeltaT += deltaT;
                    while (instance.DeltaT > frameTime)
                    {
                        instance.DeltaT -= frameTime;
                        instance.System.Update(state);
                    }
                }
            }
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
        }

    }
}
