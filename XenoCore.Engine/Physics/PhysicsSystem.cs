using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Console;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Events;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Screens;
using XenoCore.Engine.Threading;
using XenoCore.Engine.Utilities;
using XenoCore.Engine.World;

namespace XenoCore.Engine.Physics
{


    public class PhysicsSystem : ComponentSystem, IUpdateableSystem
    {
        private WorldSystem worldSystem;

        private ComponentContainer<DynamicComponent> dynamicComponents = new ComponentContainer<DynamicComponent>(EntitySystem.MAX_ENTITIES / 4);

        private ComponentContainer<CollisionComponent> collisionComponents = new ComponentContainer<CollisionComponent>(EntitySystem.MAX_ENTITIES);

        private ListArray<CollisionData> collisions = new ListArray<CollisionData>(EntitySystem.MAX_ENTITIES / 4);
        private List<DynamicComponent> updateDynamicComponents = new List<DynamicComponent>();

        private BinaryTree<CollisionComponent> collisionComponentTree;

        private ListArray<CollisionInfo> collisionEventInfo = new ListArray<CollisionInfo>(EntitySystem.MAX_ENTITIES / 4);

        private PerThreadData<PhysicsThreadData> threadData;

        private List<CollisionComponent> newComponents = new List<CollisionComponent>();
        private List<CollisionInfo> newCollisionEvents = new List<CollisionInfo>();

        private float deltaT;


        [ConsoleVariable(Name = "phys_iterations")]
        public int MaxCollisionIterations { get; set; } = 64;
        public int MinSleepVelocitySquared { get; set; } = 35;

        public IEnumerable<CollisionComponent> CollisionComponents { get { return collisionComponents.Take(collisionComponents.Count); } }

        public const int UPDATE_ORDER = UpdatingOrder.PHYSICS;

        public int UpdateOrder { get { return UPDATE_ORDER; } }


        public uint UpdatesPerSecond
        {
            get { return 60; }
        }

        private const int COLLISION_GROUPS = 16;
        public CollisionGroup[][] CollisionGroups { get; private set; }


        private EventSystem eventSystem;

        public PhysicsSystem(EntitySystem es, WorldSystem worldSystem, EventSystem eventSystem) : base(es)
        {
            this.eventSystem = eventSystem;
            this.worldSystem = worldSystem;
            threadData = new PerThreadData<PhysicsThreadData>();
            collisionComponentTree = new BinaryTree<CollisionComponent>(worldSystem.WorldSize);

            CollisionGroups = new CollisionGroup[COLLISION_GROUPS][];
            for (int j = 0; j < CollisionGroups.Length; ++j)
            {
                CollisionGroups[j] = new CollisionGroup[COLLISION_GROUPS];

                for (int i = 0; i < CollisionGroups[j].Length; ++i)
                {
                    CollisionGroups[j][i].CanCollide = true;
                    CollisionGroups[j][i].ResolveAction = CollisionResolveAction.Reflect;
                }
            }

            ConsoleService.LoadFromObject(this);
        }

        public DynamicComponent AddDynamicComponent(uint entityID)
        {
            var component = dynamicComponents.New(entityID);
            component.Reset();

            EntitySystem.RegisterComponentForEntity(this, component, entityID);

            return component;
        }
        public CollisionComponent AddCollisionComponent(uint entityID)
        {
            var component = collisionComponents.New(entityID);
            component.Reset();

            EntitySystem.RegisterComponentForEntity(this, component, entityID);
            newComponents.Add(component);

            return component;
        }
        public void RemoveDynamicComponent(uint entityID)
        {
            var component = dynamicComponents.Remove(entityID);
            EntitySystem.UnregisterComponentForEntity(this, component, entityID);
        }
        public void RemoveCollisionComponent(uint entityID)
        {
            var component = collisionComponents.Remove(entityID);
            EntitySystem.UnregisterComponentForEntity(this, component, entityID);
        }

        public DynamicComponent GetDynamicComponent(uint entityID)
        {
            return dynamicComponents.TryGetByEntityId(entityID);
        }
        public CollisionComponent GetCollisionComponent(uint entityID)
        {
            return collisionComponents.TryGetByEntityId(entityID);
        }

        public override void OnEntityDestroyed(Component systemComponent)
        {
            if (systemComponent is DynamicComponent)
            {
                var c = systemComponent as DynamicComponent;
                dynamicComponents.Remove(c);
            }
            else
            {
                var c = systemComponent as CollisionComponent;
                collisionComponents.Remove(c);
                collisionComponentTree.Remove(c._treeObject);
            }
        }

        int z = 0;

        public void Update(UpdateState state)
        {
            deltaT = state.DeltaT;

            if (state.Paused)
                return;

            InitializationStep();

            int iteration = 0;
            do
            {
                collisionEventInfo.Clear();
                collisions.Clear();
                newCollisionEvents.Clear();

                SimulationStep();

                var task = JobServiceExtender.RunJobsBatched(updateDynamicComponents.Count, 10, GenerateCollisions);
                JobService.WaitForJobs(task);

                ResolveCollisions();

                ApplySimulationStep();

                EnqueueEvents();

                ++iteration;
            }
            while (collisions.Count > 0 && iteration < MaxCollisionIterations);
        }


        private void InitializationStep()
        {
            updateDynamicComponents.Clear();

            foreach (var component in newComponents)
            {

                var worldComponent = worldSystem.GetComponent(component.Entity.ID);
                component._treeObject.BoundingBox.Size = worldComponent.ActualSize;
                component._treeObject.BoundingBox.Center = worldComponent.Position;
                collisionComponentTree.Add(component._treeObject);
            }

            newComponents.Clear();

            for (int i = 0; i < dynamicComponents.Count; ++i)
            {
                var component = dynamicComponents[i];
                if (component.Entity.ParentID > 0)
                {
                    component._energy = 0;
                    component._usedEnergy = 0;
                    component.IsAwake = false;
                    continue;
                }
                else
                {
                    component._energy = 1.0f;
                    component._usedEnergy = 0;
                }

                component.IsAwake = component.Velocity.LengthSquared() > MinSleepVelocitySquared;
                if (component.IsAwake)
                    updateDynamicComponents.Add(component);
            }

            //TODO 90% of this should be static, dont update them
            for (int i = 0; i < collisionComponents.Count; ++i)
            {
                var component = collisionComponents[i];
                var worldComponent = worldSystem.GetComponent(component.Entity.ID);
                component._boundingBox.Size = worldComponent.ActualSize;
                component._boundingBox.Center = worldComponent.Position;
                component._treeObject.BoundingBox = component._boundingBox;

                collisionComponentTree.Update(component._treeObject);
            }
        }
        private void SimulationStep()
        {
            foreach (var component in updateDynamicComponents)
            {
                var worldComponent = worldSystem.GetComponent(component.Entity.ID);

                var time = deltaT * component._energy;
                component._usedEnergy = component._energy;

                component._simVelocity = (component.Velocity + component.Acceleration * time) * (1.0f - component.Drag * 6 * time);
                component._simPosition = (worldComponent.Position + component._simVelocity * time);

                CollisionComponent collisionComponent = collisionComponents.TryGetByEntityId(component.Entity.ID);
                if (collisionComponent != null)
                {
                    collisionComponent._boundingBox.Center = component._simPosition;
                    collisionComponentTree.Update(collisionComponent._treeObject);
                }
            }
        }
        private void GenerateCollisions(Object param)
        {
            //TODO: Use different strategy : generate collision pairs and resovle them

            var threadData = this.threadData.Current;

            BatchedJobData data = (BatchedJobData)param;

            for (int i = data.StartIndex; i < data.EndIndex; ++i)
            {
                DynamicComponent dynamicComponent = updateDynamicComponents[i];
                CollisionComponent component = collisionComponents.TryGetByEntityId(dynamicComponent.Entity.ID);

                //Only collision components can collide
                if (component == null)
                    continue;

                threadData.BroadPhaseResults.Clear();
                GetBroadPhaseColliders(component, threadData);

                Vector2 movement = dynamicComponent._simVelocity * deltaT;


                if (movement.LengthSquared() == 0)
                    continue;

                if (threadData.BroadPhaseResults.Count > 0)
                    GenerateSweepPhaseCollisions(component, ref movement, threadData);
            }
        }
        private void ResolveCollisions()
        {
            for (int i = 0; i < collisions.Count; ++i)
            {
                var collision = collisions[i];

                // var time = deltaT * collision.EntryTime;

                //TODO: empty collisions are being detected

                //Known Issue : with two dynamic objects collision entry time is too early.

                var dynamicComponent = dynamicComponents.GetByEntityId(collision.ColliderID);
                var worldComponent = worldSystem.GetComponent(collision.ColliderID);
                dynamicComponent._usedEnergy = dynamicComponent._energy * collision.EntryTime;

                Vector2 collisionVelocity = dynamicComponent._simVelocity;

                dynamicComponent._simPosition = worldComponent.Position + dynamicComponent._simVelocity * collision.EntryTime * deltaT;// * 2;

                float restitution = collision.ColliderShape.Restitution * collision.ColliderShape.Restitution;

                switch (collision.Action)
                {
                    case CollisionResolveAction.None:
                        break;
                    case CollisionResolveAction.Reflect:
                        dynamicComponent._simVelocity = Vector2.Reflect(dynamicComponent._simVelocity, collision.Normal) * restitution;
                        break;
                    case CollisionResolveAction.Stop:
                        dynamicComponent._simVelocity = Vector2.Zero;
                        dynamicComponent._usedEnergy = 1.0f;
                        break;
                    case CollisionResolveAction.Slide:
                        float length = dynamicComponent._simVelocity.Length();

                        var mul = Vector2.UnitX;
                        if (collision.Normal.X != 0)
                            mul = Vector2.UnitY;
                        dynamicComponent._simVelocity *= mul * restitution;
                        if (dynamicComponent._simVelocity.LengthSquared() > 0)
                        {
                            dynamicComponent._simVelocity.Normalize();
                            dynamicComponent._simVelocity *= length;
                        }
                        break;
                }

                var eventInfo = collisionEventInfo.New();
                eventInfo.LoadFromData(collision);
                eventInfo.CollisionTime = (1.0f - dynamicComponent._energy) * deltaT;
                eventInfo.ColliderVelocity = collisionVelocity;
                newCollisionEvents.Add(eventInfo);
            }
        }
        private void ApplySimulationStep()
        {
            foreach (var component in updateDynamicComponents)
            {

                var worldComponent = worldSystem.GetComponent(component.Entity.ID);
                component.Velocity = component._simVelocity;
                worldComponent.Position = component._simPosition;
                component._energy -= component._usedEnergy;
                if (component._energy < 0)
                    component._energy = 0;

                CollisionComponent collision = collisionComponents.TryGetByEntityId(component.Entity.ID);
                if (collision != null)
                {
                    var obj = collision._treeObject;

                    obj.BoundingBox.Center = worldComponent.Position;

                    collisionComponentTree.Update(obj);
                }
            }
        }

        private void EnqueueEvents()
        {
            for (int i = 0; i < newCollisionEvents.Count; ++i)
            {
                var eventInfo = collisionEventInfo[i];
                eventSystem.EnqueueEvent<CollisionEvent>(this, eventInfo);

                bool mirrorEventExists = false;

                for (int j = 0; j < newCollisionEvents.Count; ++j)
                {
                    if (i == j)
                        continue;

                    var mirrorEventInfo = collisionEventInfo[j];

                    if (mirrorEventInfo.ColliderShape == eventInfo.CollidesWithShape &&
                        mirrorEventInfo.CollidesWithShape == eventInfo.ColliderShape)
                    {
                        mirrorEventExists = true;
                        break;
                    }

                }

                if (!mirrorEventExists)
                {
                    //The collided with object was not moving, thus no collision was found for him

                    var mirrorEventInfo = collisionEventInfo.New();
                    mirrorEventInfo.ColliderID = eventInfo.CollidesWithID;
                    mirrorEventInfo.CollidesWithID = eventInfo.ColliderID;
                    mirrorEventInfo.ColliderShape = eventInfo.CollidesWithShape;
                    mirrorEventInfo.CollidesWithShape = eventInfo.ColliderShape;
                    mirrorEventInfo.CollisionTime = eventInfo.CollisionTime;
                    mirrorEventInfo.ColliderVelocity = Vector2.Zero;

                    eventSystem.EnqueueEvent<CollisionEvent>(this, mirrorEventInfo);
                }
            }


        }

        private void GenerateSweepPhaseCollisions(CollisionComponent component, ref Vector2 moved, PhysicsThreadData threadData)
        {
            SweptAABBResult result = new SweptAABBResult()
            {
                EntryTime = 1.0f
            };

            SweptAABBResult tmpResult = new SweptAABBResult();

            bool collision = false;
            CollisionComponent collidedWith = null;
            Shape collidedShape = null;
            Shape collidedWithShape = null;



            foreach (var s in component.Shapes)
            {
                var shape = s as BoxShape;

                var bb = shape.bb;
                bb.Offset(component._boundingBox.Center);

                foreach (CollisionComponent collider in threadData.BroadPhaseResults)
                {
                    Vector2 colliderMoved = Vector2.Zero;

                    DynamicComponent dynamic = dynamicComponents.TryGetByEntityId(collider.Entity.ID);

                    if (dynamic != null && dynamic.IsAwake)
                        colliderMoved = dynamic._simVelocity * deltaT;

                    foreach (var cs in collider.Shapes)
                    {
                        var colliderShape = cs as BoxShape;

                        var cbb = colliderShape.bb;
                        cbb.Offset(collider._boundingBox.Center);

                        bool collided = SweptTest.SweptAABBTest(ref bb, ref cbb, ref moved, ref colliderMoved, ref tmpResult);

                        if (collided)
                        {
                            if (result.EntryTime > tmpResult.EntryTime)
                                result = tmpResult;

                            collidedWith = collider;
                            collidedShape = shape;
                            collidedWithShape = colliderShape;
                            collision = true;
                        }
                    }
                }
            }

            if (collision)
            {
                var collisionData = collisions.NewInterlocked();
                collisionData.ColliderID = component.Entity.ID;
                collisionData.CollidesWithID = collidedWith.Entity.ID;
                collisionData.ColliderShape = collidedShape;
                collisionData.CollidesWithShape = collidedWithShape;
                collisionData.Normal = result.Normal;
                collisionData.EntryTime = result.EntryTime;
                collisionData.Action = CollisionGroups[component.GroupID][collidedWith.GroupID].ResolveAction;
            }
        }
        private void GetBroadPhaseColliders(CollisionComponent tester, PhysicsThreadData threadData)
        {
            threadData.TreeList.Clear();
            threadData.TreeList.AddRange(collisionComponents.Take(collisionComponents.Count));
            // collisionComponentTree.IntersectRectangle(ref tester._boundingBox, threadData.TreeList);

            foreach (var collider in threadData.TreeList)
            {
                if (collider == tester)
                    continue;

                bool canCollide = CollisionGroups[tester.GroupID][collider.GroupID].CanCollide;

                if (canCollide && collider._boundingBox.Intersects(tester._boundingBox))
                    threadData.BroadPhaseResults.Add(collider);
            }

        }

        public void GetEntitiesInRegion(RectangleF rect, List<uint> outItems)
        {
            var threadData = this.threadData.Current;
            threadData.TreeList.Clear();
            collisionComponentTree.IntersectRectangle(ref rect, threadData.TreeList);

            bool result = false;

            foreach (var item in threadData.TreeList)
            {
                item._boundingBox.Intersects(ref rect, out result);
                if (result)
                    outItems.Add(item.Entity.ID);
            }
        }
        public void GetEntitiesInCircle(Vector2 pos, float radius, List<uint> outItems)
        {
            RectangleF rect = new RectangleF(pos - Vector2.One * radius, pos + Vector2.One * radius);
            CircleF circle = new CircleF(pos, radius);

            var threadData = this.threadData.Current;
            threadData.TreeList.Clear();
            collisionComponentTree.IntersectRectangle(ref rect, threadData.TreeList);

            bool result = false;

            foreach (var item in threadData.TreeList)
            {
                circle.Intersects(ref rect, out result);
                if (result)
                    outItems.Add(item.Entity.ID);
            }
        }

        public void Dispose()
        {
            ConsoleService.UnloadFromObject(this);
        }
    }
}
