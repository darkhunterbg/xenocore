using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Utilities
{

    public class BinaryTreeObject<T> where T : class
    {
        public RectangleF BoundingBox;
        internal BinaryTreeNode<T> Node;
        public T Object;
    }

    class BinaryTreeNode<T> where T : class
    {
        public const int MAX_OBJECTS = 128;

        internal List<BinaryTreeObject<T>> Objects = new List<BinaryTreeObject<T>>(MAX_OBJECTS);
        public BinaryTreeNode<T> Negative = null;
        public BinaryTreeNode<T> Positive = null;
        public BinaryTreeNode<T> Parent = null;

        public int Depth = 0;

        public RectangleF BoundingRect = new RectangleF();

        public bool IsVerticalSplit = false;
    }

    public class BinaryTree<T> where T : class
    {
        private BinaryTreeNode<T> root;
        private Vector2 size;

        private int count = 0;

        public BinaryTree(Point size) : this(size.X, size.Y) { }
        public BinaryTree(int width, int height)
        {
            size = new Vector2(width, height);
            root = new BinaryTreeNode<T>()
            {
                BoundingRect = new RectangleF(-size / 2, size / 2),
                IsVerticalSplit = true,
            };
            PartitionNode(root);
        }

        public void Add(BinaryTreeObject<T> obj)
        {
            Debug.Assert(root.BoundingRect.Contains(obj.BoundingBox), "Object outside the world!");
            Add(obj, root);

            ++count;
        }
        private void Add(BinaryTreeObject<T> obj, BinaryTreeNode<T> node)
        {
            BinaryTreeNode<T> addNode = node;

            do
            {
                addNode = AddIterativly(obj, addNode);
            }
            while (addNode != null);


        }
        private BinaryTreeNode<T> AddIterativly(BinaryTreeObject<T> obj, BinaryTreeNode<T> node)
        {
            Vector2 position = obj.BoundingBox.Center;

            bool neg, pos;
            node.Negative.BoundingRect.Intersects(ref obj.BoundingBox, out neg);
            node.Positive.BoundingRect.Intersects(ref obj.BoundingBox, out pos);
            if (neg && pos)
            {
                obj.Node = node;
                node.Objects.Add(obj);
                return null;
            }

            BinaryTreeNode<T> addNode = null;

            if (node.IsVerticalSplit)
            {
                if (position.X < node.BoundingRect.Center.X)
                    addNode = node.Negative;
                else
                    addNode = node.Positive;
            }
            else
            {
                if (position.Y < node.BoundingRect.Center.Y)
                    addNode = node.Negative;
                else
                    addNode = node.Positive;
            }

            //Has partitioning been used?
            if (addNode.Negative == null)
            {
                //No partitioning has been used, should we split?
                if (addNode.Objects.Count == BinaryTreeNode<T>.MAX_OBJECTS)
                {
                    PartitionNode(addNode);
                    var items = addNode.Objects.ToList();
                    addNode.Objects.Clear();

                    for (int i = 0; i < items.Count; ++i)
                    {
                        RectangleF rect = items[i].BoundingBox;
                        Add(items[i], addNode);
                    }


                    return addNode;
                }
                else
                {
                    obj.Node = addNode;
                    addNode.Objects.Add(obj);
                    return null;
                }
            }
            else
            {
                return addNode;
            }
        }

        public void Remove(BinaryTreeObject<T> obj)
        {
            obj.Node.Objects.Remove(obj);
            --count;
        }


        public void IntersectRectangle(ref RectangleF rect, List<T> result)
        {
            IntersectRectangle(ref rect, root, result);
        }
        private void IntersectRectangle(ref RectangleF rect, BinaryTreeNode<T> node, List<T> output)
        {
            bool result = false;
            rect.Intersects(ref node.BoundingRect, out result);
            if (result)
            {
                for (int i = 0; i < node.Objects.Count; ++i)
                    output.Add(node.Objects[i].Object);


                if (node.Negative != null)
                {
                    IntersectRectangle(ref rect, node.Negative, output);
                    IntersectRectangle(ref rect, node.Positive, output);
                }
            }
        }

        public void Update(BinaryTreeObject<T> obj)
        {
            bool result = false;
            obj.Node.BoundingRect.Contains(ref obj.BoundingBox, out result);
            if (!result)
            {
                obj.Node.Objects.Remove(obj);
                Add(obj);
            }
        }


        private void PartitionNode(BinaryTreeNode<T> node)
        {
            ++node.Depth;
            if (node.Parent != null)
                node.Parent.Depth += 1;

            if (node.IsVerticalSplit)
            {
                node.Negative = new BinaryTreeNode<T>();
                node.Negative.BoundingRect.Min.X = node.BoundingRect.Min.X;
                node.Negative.BoundingRect.Width = node.BoundingRect.Width / 2;
                node.Negative.BoundingRect.Min.Y = node.BoundingRect.Min.Y;
                node.Negative.BoundingRect.Height = node.BoundingRect.Height;

                node.Positive = new BinaryTreeNode<T>();
                node.Positive.BoundingRect.Min.X = node.BoundingRect.Min.X + node.BoundingRect.Width / 2;
                node.Positive.BoundingRect.Width = node.BoundingRect.Width / 2;
                node.Positive.BoundingRect.Min.Y = node.BoundingRect.Min.Y;
                node.Positive.BoundingRect.Height = node.BoundingRect.Height;

                node.Negative.IsVerticalSplit = node.Positive.IsVerticalSplit = false;
            }
            else
            {
                node.Negative = new BinaryTreeNode<T>();
                node.Negative.BoundingRect.Min.X = node.BoundingRect.Min.X;
                node.Negative.BoundingRect.Width = node.BoundingRect.Width;
                node.Negative.BoundingRect.Min.Y = node.BoundingRect.Min.Y;
                node.Negative.BoundingRect.Height = node.BoundingRect.Height / 2;

                node.Positive = new BinaryTreeNode<T>();
                node.Positive.BoundingRect.Min.X = node.BoundingRect.Min.X;
                node.Positive.BoundingRect.Width = node.BoundingRect.Width;
                node.Positive.BoundingRect.Min.Y = node.BoundingRect.Min.Y + node.BoundingRect.Height / 2; ;
                node.Positive.BoundingRect.Height = node.BoundingRect.Height / 2;

                node.Negative.IsVerticalSplit = node.Positive.IsVerticalSplit = true;
            }

            node.Negative.Parent = node;
            node.Positive.Parent = node;
        }
    }
}
