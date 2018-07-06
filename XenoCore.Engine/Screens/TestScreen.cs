using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using XenoCore.Engine.World;
using XenoCore.Engine.Resources;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Console;
using XenoCore.Engine.Input;
using Microsoft.Xna.Framework.Input;
using XenoCore.Engine.Physics;
using XenoCore.Engine.Events;
using XenoCore.Engine.Scripting;
using XenoCore.Engine.GUI;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Screens
{
    public class CustomEvent : Event<TestScreen, int> { }

    public class TestScreen : WorldScreen, IEventReciever
    {

        public override void UpdateInput(GameTime gameTime)
        {
            base.UpdateInput(gameTime);

            Vector2 movement = Vector2.Zero;
            if (InputService.InputState.CurrentInput.Keyboard.IsKeyDown(Keys.W))
                movement += new Vector2(0, -1);
            if (InputService.InputState.CurrentInput.Keyboard.IsKeyDown(Keys.S))
                movement += new Vector2(0, 1);
            if (InputService.InputState.CurrentInput.Keyboard.IsKeyDown(Keys.A))
                movement += new Vector2(-1, 0);
            if (InputService.InputState.CurrentInput.Keyboard.IsKeyDown(Keys.D))
                movement += new Vector2(1, 0);

            if (InputService.InputState.WasKeyReleased(Keys.OemPlus))
                CameraSystem.CameraZoom *= 2.0f;

            if (InputService.InputState.WasKeyReleased(Keys.OemMinus))
                CameraSystem.CameraZoom /= 2.0f;

            if (movement.LengthSquared() == 0)
                return;

            movement.Normalize();

            CameraSystem.CurrentCamera.Position += movement * 3000 * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public TestScreen() :
            base(new Point(1024 * 1024, 1024 * 1024))
        {
            //eventSystem.AddReciever<CollisionEvent>(this);
            //eventSystem.AddReciever<CustomEvent>(this);

            ScriptingSystem.LoadFromAssembly("XenoCore.Engine");

            CameraSystem.Cameras[0].WorldViewport = new Vector2(1280, 720);
            CameraSystem.Cameras.Add(new Camera(new Vector2(1280, 720)));

            //    GUISystem.Debug = true;

            var container = new GUI.StackContainer();
            container.Orientation = StackOrientation.Horizontal;
            //container.Rows.Add(new GridContainer.GridRow(0.5f));
            //container.Rows.Add(new GridContainer.GridRow(0.5f));
            //container.Columns.Add(new GridContainer.GridColumn(0.5f));
            //container.Columns.Add(new GridContainer.GridColumn(0.5f));


            container.Children.Add(new CameraDisplay()
            {
                Camera = CameraSystem.Cameras[0],
                Weight = 0.5f,
                Visiblity = Visiblity.Visible

                //  Width = new Coordinate(640),
                //  Height = new Coordinate(0.5f),
                // HorizontalAlignment = HorizontalAlignment.Left,
            });

            //container.Children.Add(new CameraDisplay()
            //{
            //    Camera = CameraSystem.Cameras[1],
            //    Weight = 0.5f,
            //    // Width = new Coordinate(640),
            //    // Height = new Coordinate(0.5f),
            //    // HorizontalAlignment = HorizontalAlignment.Right,
            //});

            //for (int i = 0; i < 10; ++i)
            //{
            //    var stack = new StackContainer()
            //    {
            //        Orientation = StackOrientation.Vertical,
            //        Weight = 1,
            //    };
            //    for (int j = 0; j < 10; ++j)
            //    {
            //        stack.Children.Add(new Button()
            //        {
            //            Margin = new MarginDef(1),
            //            Weight = 1,
            //            Text = ".",
            //            Clicked = () => ConsoleService.DevMsg("Button Clicked!"),
            //        });
            //    }

            //    container.Children.Add(stack);
            //}


            PhysicsDebugger.Enabled = true;

            GUISystem.RootControl = container;

            //var effect = ResourcesService.LoadParticleEffect("effect");
            //  ParticleSystem.AddParticleEffect(effect);



            var effect = new ParticleEffectDescription()
            {
                Name = "test"
            };

            effect.Emitters.Add(new ParticleEmitterDescription("test")
            {
                MaxParticles = 1000,
            });
            effect.Emitters[0].Modules.Add(new RequiredModule());
            effect.Emitters[0].Required.Blending = BlendingMode.Additive;
            effect.Emitters[0].Required.TextureName = "Wave";
            //effect.Emitters[0].Required.TexturePart = new Rectangle(0, 0, 119, 119);
            effect.Emitters[0].Required.Duration = 2.0f;
            effect.Emitters[0].Required.Delay = 0.1f;
            effect.Emitters[0].Modules.Add(new SpawnBurstModule()
            {
                BurstCount = 16
            });
            effect.Emitters[0].Modules.Add(new ConeVelocityModule()
            {
                Angle = 1.0f,
                Velocity = new RandomFloat(300)
            });
            effect.Emitters[0].Modules.Add(new LifetimeModule()
            {
                Lifetime = new RandomFloat(1.0f)
            });
            effect.Emitters[0].Modules.Add(new ColorInitModule()
            {
                StartColor = new RandomColor(Color.Red),
            });
            effect.Emitters[0].Modules.Add(new ColorOverLifeModule()
            {
                ColorOverLife = new RandomColor(Color.Blue, Color.Green),
                 AlphaOverLife = new RandomFloat(0),
            });
            //effect.Emitters[0].Modules.Add(new InitialVelocityModule()
            //{
            //    StartVelocity = 300
            //});
            effect.Emitters[0].Modules.Add(new InitialRotatioRateModule()
            {
                StartRotationRate = new RandomFloat(0.01f),
            });
            effect.Emitters[0].Modules.Add(new RotationRateLifeScaleModule()
            {
                LifeMultiplier = new RandomFloat(0, 10),
            });

            ParticleSystem.AddParticleEffect(effect);


            ConsoleService.LoadFromObject(this);

            PhysicsSystem.CollisionGroups[0][0].CanCollide = true;
            PhysicsSystem.CollisionGroups[0][0].ResolveAction = CollisionResolveAction.Reflect;
            //Paused = true;
            Restart();
        }

        [ConsoleCommand(Name = "event")]
        public void SendEvent(int param)
        {
            EventSystem.EnqueueEvent<CustomEvent>(this, param);
        }

        [ConsoleCommand(Name = "stack")]
        public void Stack(Visiblity visibility)
        {
            var control = (GUISystem.RootControl as GUIContainer).Children[0].Visiblity = visibility;
        }

        [ConsoleCommand(Name = "restart")]
        public void Restart()
        {
            var e = EntitySystem.GetAllEntities();
            for (int i = 0; i < e.Count; ++i)
            {
                EntitySystem.DeleteEntity(e[i].ID);
            }

            for (int i = 0; i < 500; ++i)
            {
                var entity = EntitySystem.NewEntity();
                var c = WorldSystem.AddComponent(entity);

                c.Position = new Vector2(i % 100, (i / 100)) * new Vector2(150, 180);// + new Vector2(-200, -200);
                c.BaseSize = new Vector2(89, 150);
                c.Texture = GraphicsService.Cache.GetTexture("Player");
                c.Color = Color.White;
                c.Blending = Graphics.BlendingMode.Alpha;

                var p = ParticleSystem.AddComponent(entity, "test");

                var ph = PhysicsSystem.AddDynamicComponent(entity);
                var col = PhysicsSystem.AddCollisionComponent(entity);
                col.GroupID = 0;
                col.Shapes.Add(new BoxShape(c.BaseSize) { Restitution = 0.95f });

                ph.Drag = 0.15f;
                ph.Velocity = new Vector2(-800, 00);// * ((2*i)-1); // * 8;
                ph.Acceleration = new Vector2(0, 10);

                //var entity2 = EntitySystem.NewEntity();
                //var c2 = WorldSystem.AddComponent(entity2);

                //c2.BaseSize = new Vector2(89, 150);
                //c2.Texture = GraphicsService.Cache.GetTexture("Player");
                //c2.Color = Color.Red;
                //c2.Blending = Graphics.BlendingMode.Alpha;
                //c2.ParentOffset = new Vector2(0, 50);

                //EntitySystem.AddChild(entity, entity2);
            }

            var entity1 = EntitySystem.NewEntity();
            var c1 = WorldSystem.AddComponent(entity1);

            c1.Position = new Vector2(-400, 1000);
            c1.BaseSize = new Vector2(89, 3000);
            c1.Texture = GraphicsService.Cache.GetTexture("Player");
            c1.Color = Color.White;
            c1.Blending = Graphics.BlendingMode.Alpha;

            var col1 = PhysicsSystem.AddCollisionComponent(entity1);
            col1.Shapes.Add(new BoxShape(c1.BaseSize));
            col1.GroupID = 1;
            //var ph1 = physicsSystem.AddDynamicComponent(entity1);
            // ph1.Velocity = new Vector2(300, 0);
        }



        public override void Dispose()
        {
            base.Dispose();
            ConsoleService.UnloadFromObject(this);
        }

        public void OnEventDispatched(Event e)
        {
            var ev = e.Cast<CollisionEvent>();

            ConsoleService.DevMsg($"{ev.Argument.ColliderID} collides with {ev.Argument.CollidesWithID}");
            //   ConsoleService.DevMsg($"{e.EventType.Name} from: {e.Sender} ; param: {e.Argument}");
        }
    }


    [Script(Name = "test")]
    public class TestScript : Script
    {
        public override IEnumerator<WaitTrigger> Action()
        {
            ConsoleService.DevMsg($"Script Triggered!");

            yield return WaitTrigger.WaitTime(1.0f, Systems);
            //  yield return WaitTrigger.WaitForEvent<CustomEvent>();

            ConsoleService.DevMsg($"Script Re-Triggered!");

            yield return null;
        }

        public override Condition[] GetConditions()
        {
            return new Condition[] { (context) =>
            {
                return context.TriggerEvent.Cast<CustomEvent>().Argument == 10;

            } };
        }

        public override Trigger[] GetTriggers()
        {
            return new Trigger[] { new Trigger(typeof(CustomEvent)) };
        }
    }
}
