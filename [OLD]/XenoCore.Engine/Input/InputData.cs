using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Input
{
    public class InputAction
    {
        public bool Enabled { get; set; } = true;
        public Func<InputState, bool> Condition { get; set; }

        public InputAction() { }
        public InputAction(Func<InputState, bool> cond)
        {
            this.Condition = cond;
        }
    }

    public class InputData
    {
        public KeyboardState Keyboard { get; set; }
        public MouseState Mouse { get; set; }
        public GamePadState[] GamePads { get; set; } = new GamePadState[4];

        public ulong Frame { get; set; }

        internal float backspace_counter = 0;
    }

    public class InputState
    {
        public InputData PreviousInput { get; internal set; } = new InputData();
        public InputData CurrentInput { get; internal set; } = new InputData();


        internal List<char> CharInputQueue { get; private set; } = new List<char>();

        public bool WasKeyReleased(Keys key)
        {
            return PreviousInput.Keyboard.IsKeyDown(key) && CurrentInput.Keyboard.IsKeyUp(key);
        }
        public bool WasKeyPressed(Keys key)
        {
            return PreviousInput.Keyboard.IsKeyUp(key) && CurrentInput.Keyboard.IsKeyDown(key);
        }
        public bool WasMouseButtonReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return PreviousInput.Mouse.LeftButton == ButtonState.Pressed &&
                           CurrentInput.Mouse.LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return PreviousInput.Mouse.RightButton == ButtonState.Pressed &&
                           CurrentInput.Mouse.RightButton == ButtonState.Released;

                case MouseButton.Middle:
                    return PreviousInput.Mouse.MiddleButton == ButtonState.Pressed &&
                           CurrentInput.Mouse.MiddleButton == ButtonState.Released;
            }
            return false;
        }
        public bool WasMouseButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return PreviousInput.Mouse.LeftButton == ButtonState.Released &&
                           CurrentInput.Mouse.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return PreviousInput.Mouse.RightButton == ButtonState.Released &&
                           CurrentInput.Mouse.RightButton == ButtonState.Pressed;

                case MouseButton.Middle:
                    return PreviousInput.Mouse.MiddleButton == ButtonState.Released &&
                           CurrentInput.Mouse.MiddleButton == ButtonState.Pressed;
            }
            return false;
        }


        public void UpdateInputText(StringBuilder sb, params char[] ignore)
        {
            foreach (var c in CharInputQueue)
            {
                if (!ignore.Contains(c))
                    sb.Append(c);
            }

            if (sb.Length > 0)
            {
                if (WasKeyPressed(Keys.Back))
                {
                    --sb.Length;
                }
                else while (CurrentInput.backspace_counter > 0.2f)
                    {
                        --sb.Length;
                        CurrentInput.backspace_counter -= 0.02f;
                    }
            }
        }
    }
}
