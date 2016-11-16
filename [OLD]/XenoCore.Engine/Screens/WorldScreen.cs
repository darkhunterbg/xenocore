using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Events;
using XenoCore.Engine.GUI;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Physics;
using XenoCore.Engine.Scripting;
using XenoCore.Engine.World;

namespace XenoCore.Engine.Screens
{
    public abstract class WorldScreen : Screen
    {
        public EntitySystem EntitySystem { get; private set; }
        public WorldSystem WorldSystem { get; private set; }
        public PhysicsSystem PhysicsSystem { get; private set; }
        public ScriptingSystem ScriptingSystem { get; private set; }
        public TimingSystem TimingSystem { get; private set; }

        public GUISystem GUISystem { get; private set; }

        public CameraSystem CameraSystem { get; private set; }
        public ParticleSystem ParticleSystem { get; private set; }

        public EventSystem EventSystem { get; private set; }

        public PhysicsDebugger PhysicsDebugger { get; private set; }

        public WorldScreen(Point worldSize)
        {
            Systems.Register(EntitySystem = new EntitySystem());
            Systems.Register(EventSystem = new EventSystem());

            Systems.Register(CameraSystem = new CameraSystem());

            Systems.Register(WorldSystem = new WorldSystem(EntitySystem,CameraSystem, worldSize));
            Systems.Register(ParticleSystem = new ParticleSystem(EntitySystem, WorldSystem, CameraSystem));
            Systems.Register(PhysicsSystem = new PhysicsSystem(EntitySystem, WorldSystem, EventSystem));

            Systems.Register(TimingSystem = new TimingSystem(EventSystem, EntitySystem));
            Systems.Register(ScriptingSystem = new ScriptingSystem(EventSystem, Systems));

            Systems.Register(GUISystem = new GUISystem());

            Systems.Register(PhysicsDebugger = new PhysicsDebugger(CameraSystem, PhysicsSystem, WorldSystem));

        }

        public override void UpdateInput(GameTime gameTime)
        {
            base.UpdateInput(gameTime);
            GUISystem.UpdateInput();
        }
    }
}
