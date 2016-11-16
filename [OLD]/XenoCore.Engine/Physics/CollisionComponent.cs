using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Physics
{
    public class CollisionComponent : Component
    {
        public List<Shape> Shapes { get; private set; } = new List<Shape>();

        internal BinaryTreeObject<CollisionComponent> _treeObject = new BinaryTreeObject<CollisionComponent>();
        internal RectangleF _boundingBox;

        public int GroupID = 0;

        public CollisionComponent()
        {
            _treeObject.Object = this;
        }

        public void Reset()
        {
            GroupID = 0;
            Shapes.Clear();
        }
    }
}
