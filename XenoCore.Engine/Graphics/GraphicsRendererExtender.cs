using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Resources;
using XenoCore.Engine.World;

namespace XenoCore.Engine.Graphics
{
    public static class GraphicsRendererExtender
    {
        public static RenderInstance NexTextureRelativeToCamera(Camera cam,ref RectangleF rect, Texture texture,
       BlendingMode bledning, byte layer, bool cull)
        {
            return NexTextureRelativeToCamera(ref cam.ViewRect,ref rect, texture, bledning, layer, cam.RenderStateIndex, cull);
        }
        static RenderInstance NexTextureRelativeToCamera(ref RectangleF viewRect,ref RectangleF rect, Texture texture,
            BlendingMode bledning, byte layer, int stateIndex, bool cull)
        {
            Vector2 halfSize = rect.Size / 2.0f;

            if (cull && !viewRect.Intersects(rect))
                return null;

            var instance = GraphicsService.Renderer.NewTextureInterlocked(texture, layer, bledning, stateIndex);
            instance.Destination.Width = (int)rect.Width;
            instance.Destination.Height = (int)rect.Height;
            instance.Destination.X = (int)(rect.X + halfSize.X - viewRect.X);
            instance.Destination.Y = (int)(rect.Y + halfSize.Y - viewRect.Y);

            return instance;
        }

        public static void RenderLine(Camera camera, ref RectangleF lineRect, Color color, byte layer, bool cull)
        {
            RenderLine(ref camera.ViewRect, ref lineRect, color, layer, camera.RenderStateIndex, cull);
        }

        static void RenderLine(ref RectangleF viewRect, ref RectangleF lineRect, Color color, byte layer, int stateIndex, bool cull)
        {
            var i = NexTextureRelativeToCamera(ref viewRect,ref  lineRect, GraphicsService.Cache.WhiteTexture, BlendingMode.Alpha,
                layer, stateIndex, cull);
            if (i == null)
                return;

            i.Color = color;
            i.TexturePart = new Rectangle(0, 0, 1, 1);
            i.Rotation = 0;
            i.Center = Vector2.One / 2;
        }

        public static void RenderBox(ref RectangleF rect, Color color, byte layer, int stateIndex = 0)
        {
            RectangleF viewRect = new RectangleF(Vector2.Zero,
                new Vector2(GraphicsService.BackBufferSize.X, GraphicsService.BackBufferSize.Y));
            RenderBox(ref viewRect, ref rect, color, layer, stateIndex, false);
        }
        public  static void RenderBox(Camera camera, ref RectangleF rect, Color color, byte layer, bool cull)
        {
            RenderBox(ref camera.ViewRect, ref rect, color, layer, camera.RenderStateIndex, false);
        }
        static void RenderBox(ref RectangleF viewRect, ref RectangleF rect, Color color, byte layer, int stateIndex, bool cull)
        {
            var instance = GraphicsRendererExtender.NexTextureRelativeToCamera(ref viewRect,ref  rect,
                 GraphicsService.Cache.WhiteTexture, BlendingMode.Alpha, layer, stateIndex, cull);
            if (instance == null)
                return;

            instance.Color = color * 0.5f;
            instance.TexturePart = new Rectangle(0, 0, 1, 1);
            instance.Center = Vector2.One / 2;
            instance.Rotation = 0;

            RectangleF r = rect;
            r.Height = 2;
            GraphicsRendererExtender.RenderLine(ref viewRect, ref r, color * 0.75f, layer, stateIndex, false);

            r.Y += rect.Height - r.Height;
            GraphicsRendererExtender.RenderLine(ref viewRect, ref r, color * 0.75f, layer, stateIndex, false);

            r.Y = rect.Y;
            r.Height = rect.Height;
            r.Width = 2;
            GraphicsRendererExtender.RenderLine(ref viewRect, ref r, color * 0.75f, layer, stateIndex, false);

            r.Y = rect.Y;
            r.X += rect.Width - r.Width;
            GraphicsRendererExtender.RenderLine(ref viewRect, ref r, color * 0.75f, layer, stateIndex, false);
        }


    }
}
