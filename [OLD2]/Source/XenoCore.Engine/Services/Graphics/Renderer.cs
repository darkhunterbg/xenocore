using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Services.Graphics
{
    public enum BlendingMode : uint
    {
        Solid = 0,
        Alpha = 1,
        Additive = 2,
    }

    public class Renderer : IDisposable
    {
        /// Format:
        /// [Layer][Blending][Texture][Matrix]
        /// <summary>
        /// NOTE: Lower key means it will be drawn first
        /// </summary>

        const int SPRITE_FONT_OFFSET = 0x400;

        const int KEY_SIZE_BITS = sizeof(UInt32) * 8;

        const int STATE_BITS = 4;
        const int STATE_BITS_OFFSET = KEY_SIZE_BITS - STATE_BITS;

        const int LAYER_BITS = 8;
        const int LAYER_BITS_OFFSET = STATE_BITS_OFFSET - LAYER_BITS;

        const int BLEND_BITS = 2;
        const int BLEND_BITS_OFFSET = LAYER_BITS_OFFSET - BLEND_BITS;

        const int TEXTURE_BITS = 18;
        const int TEXTURE_BIT_OFFSET = BLEND_BITS_OFFSET - TEXTURE_BITS;


        const UInt32 STATE_MASK = (UInt32)((1 << STATE_BITS) - 1) << STATE_BITS_OFFSET;
        const UInt32 LAYER_MASK = (UInt32)((1 << LAYER_BITS) - 1) << LAYER_BITS_OFFSET; //0xFF000000;
        const UInt32 BLEND_MASK = (UInt32)((1 << BLEND_BITS) - 1) << BLEND_BITS_OFFSET; //0x00F00000;
        const UInt32 TEXTURE_MASK = (UInt32)((1 << TEXTURE_BITS) - 1) << TEXTURE_BIT_OFFSET; //0x000FFFFF

        readonly int[] BITS_SIZES = new int[] { STATE_BITS, LAYER_BITS, BLEND_BITS, TEXTURE_BITS };

        private GraphicsDevice device;
        private ResourceCache resourceCache;
        private SpriteBatch spriteBatch;
        private BlendState[] blendingStates;

        public RenderState[] States { get; private set; } = new RenderState[2 << STATE_BITS];

        private ListArray<RenderCommand> commandList = new ListArray<RenderCommand>(128);

        public Renderer(GraphicsDevice device, ResourceCache resourceCache)
        {
            Debug.AssertDebug(BITS_SIZES.Sum() == KEY_SIZE_BITS,
                $"Renderer key size is expected to be {KEY_SIZE_BITS}, but it was {BITS_SIZES.Sum()}!");

            this.device = device;
            this.spriteBatch = new SpriteBatch(device);
            this.resourceCache = resourceCache;

            blendingStates = new BlendState[] { BlendState.Opaque, BlendState.AlphaBlend, BlendState.Additive };

            for (int i = 0; i < States.Length; ++i)
                States[i] = new RenderState();
        }

        public void Dispose()
        {
            spriteBatch.Dispose();
        }
        public RenderInstance NewTexture(Texture texture, byte layer, BlendingMode blending, int stateID)
        {
            return NewInstance(texture.id, layer, blending, false, stateID);
        }
        public RenderInstance NewFont(Font font, byte layer, BlendingMode blending, int stateID)
        {
            return NewInstance(font.id, layer, blending, true, stateID);
        }

        private RenderCommand GetCommand(UInt32 key)
        {
            RenderCommand command = null;

            for (int i = 0; i < commandList.Count; ++i)
            {
                if (commandList[i].Key == key)
                {
                    command = commandList[i];
                    if (command.Data.Instances.Count == command.Data.Instances.Size)
                        command = null;
                    else
                        break;
                }
            }

            if (command == null)
            {
                command = commandList.New();
                command.Data.Instances.Clear();
                command.Key = key;
            }


            return command;
        }
        private RenderInstance NewInstance(int resourceID, byte layer, BlendingMode blending, bool font, int stateId)
        {
            Debug.AssertDebug(resourceID > 0, "Invalid resourceID!");

            RenderInstance result = null;

            UInt32 key = (UInt32)stateId;
            key = (key << LAYER_BITS) + (UInt32)(layer);
            key = (key << BLEND_BITS) + (UInt32)blending;
            key = (key << TEXTURE_BITS) + (UInt32)(resourceID + (font ? SPRITE_FONT_OFFSET : 0));

            RenderCommand command = GetCommand(key);
            RenderCommandData data = command.Data;
            data.IsSpriteFont = font;
            result = data.Instances.New();

            return result;
        }

        public void Clear(Color color)
        {
            device.Clear(color);
        }

        public void ExecuteCommands()
        {
            States[0].TransformMatrix = Matrix.Identity;
            //  States[0].Viewport = device.Viewport;

            //Like this for now
            var sorted = commandList.Take(commandList.Count).OrderBy(p => p.Key).ToList();

            for (int i = 0; i < commandList.Count; ++i)
            {
                var command = sorted[i];

                RenderCommandData data = command.Data;

                int blend = (int)((command.Key & BLEND_MASK) >> BLEND_BITS_OFFSET);

                BlendState blendState = blendingStates[blend];

                int stateid = (int)((command.Key & STATE_MASK) >> STATE_BITS_OFFSET);

                var state = States[stateid];

                spriteBatch.Begin(SpriteSortMode.Deferred, blendState, null, null, null, null, state.TransformMatrix);

                // device.Viewport = state.Viewport;

                if (data.IsSpriteFont)
                {
                    int id = ((int)(command.Key & TEXTURE_MASK) >> TEXTURE_BIT_OFFSET) - SPRITE_FONT_OFFSET;
                    SpriteFont font = resourceCache[new Font(id)];

                    for (int j = 0; j < data.Instances.Count; ++j)
                    {
                        var instance = data.Instances[j];
                        //Hack it for now
                        spriteBatch.DrawString(font, instance.Text, new Vector2(instance.Destination.X, instance.Destination.Y), instance.Color,
                            instance.Rotation, instance.Center, instance.TextScale, instance.Effects, 0);
                    }
                }
                else
                {
                    int id = (int)(command.Key & TEXTURE_MASK) >> TEXTURE_BIT_OFFSET;
                    Texture2D texture = resourceCache[new Texture(id)];

                    for (int j = 0; j < data.Instances.Count; ++j)
                    {
                        var instance = data.Instances[j];

                        spriteBatch.Draw(texture, instance.Destination, instance.TexturePart, instance.Color,
                            instance.Rotation, instance.Center, instance.Effects, 0);
                        /// 0,  new Vector2(instance.Destination.Width/2, instance.Destination.Height/2), SpriteEffects.None, 0);
                    }

                }

                spriteBatch.End();
                data.Instances.Clear();
            }

            commandList.Clear();

            // device.Viewport = States[0].Viewport;
        }
    }
}
