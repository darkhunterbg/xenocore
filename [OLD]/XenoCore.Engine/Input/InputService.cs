using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XenoCore.Engine.Utilities;

namespace XenoCore.Engine.Input
{

    public class InputService 
    {
        private static InputService instance;

        private InputState inputState = new InputState();
        private ulong frameCount = 0;
        private List<Char> textInputProviderQueue = new List<char>();
        private ITextInputProvider textInput;

        public static InputState InputState
        {
            get { return instance.inputState; }
        }

        private InputService() { }

        public static void Initialize(ITextInputProvider input)
        {
            Debug.Assert(instance == null, "InputService is already initialized!");

            instance = new InputService()
            {
                textInput =input
            };

            instance.textInput.OnCharacterEntered += Input_OnCharacterEntered;
        }
        public static void Uninitialize()
        {
            Debug.Assert(instance != null, "InputService is not initialized!");

            instance.textInput.OnCharacterEntered -= Input_OnCharacterEntered;
            instance = null;
        }
        public static bool IsInputActionTriggered(InputAction action)
        {
            if (!action.Enabled)
                return false;

            return action.Condition(InputState);
        }

        private static void Input_OnCharacterEntered(char c)
        {
            instance.textInputProviderQueue.Add(c);
        }

        public static void Update(GameTime time)
        {
            InputState.CharInputQueue.Clear();

            InputState.CharInputQueue.AddRange(instance.textInputProviderQueue);
            instance.textInputProviderQueue.Clear();

            var tmp = InputState.PreviousInput;
            InputState.PreviousInput = InputState.CurrentInput;
            InputState.CurrentInput = tmp;

            InputState.CurrentInput.Frame = ++instance.frameCount;
            InputState.CurrentInput.Keyboard = Keyboard.GetState();
            InputState.CurrentInput.Mouse = Mouse.GetState();

            for (int i = 0; i < InputState.CurrentInput.GamePads.Length; ++i)
                InputState.CurrentInput.GamePads[i] = GamePad.GetState((PlayerIndex)i);

            if (InputState.CurrentInput.Keyboard.IsKeyDown(Keys.Back))
            {
                InputState.CurrentInput.backspace_counter += (float)time.ElapsedGameTime.TotalSeconds;
            }
            else
                InputState.CurrentInput.backspace_counter = 0;

        }

   
    }
}
