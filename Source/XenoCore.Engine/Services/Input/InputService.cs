using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Services.Input
{
    public class InputService : IDisposable
    {
        private ITextInputProvider inputProvider;
        private List<Char> textInputProviderQueue = new List<char>();

        public InputState State { get; private set; } = new InputState();

        public InputService(ITextInputProvider inputProvider)
        {
            this.inputProvider = inputProvider;

            if (inputProvider != null)
                inputProvider.OnCharacterEntered += InputProvider_OnCharacterEntered;

            
        }

        private void InputProvider_OnCharacterEntered(char obj)
        {
            textInputProviderQueue.Add(obj);
        }

        public void Dispose()
        {
            if (inputProvider != null)
                inputProvider.OnCharacterEntered -= InputProvider_OnCharacterEntered;
        }

        public void Update(GameTime time)
        {
            State.CharInputQueue.Clear();

            State.CharInputQueue.AddRange(textInputProviderQueue);
            textInputProviderQueue.Clear();

            var tmp = State.PreviousInput;
            State.PreviousInput = State.CurrentInput;
            State.CurrentInput = tmp;

            State.CurrentInput.Keyboard = Keyboard.GetState();
            State.CurrentInput.Mouse = Mouse.GetState();

            for (int i = 0; i < State.CurrentInput.GamePads.Length; ++i)
                State.CurrentInput.GamePads[i] = GamePad.GetState((PlayerIndex)i);

            if (State.CurrentInput.Keyboard.IsKeyDown(Keys.Back))
            {
                State.CurrentInput.backspace_counter += (float)time.ElapsedGameTime.TotalSeconds;
            }
            else
                State.CurrentInput.backspace_counter = 0;
        }
    }
}
