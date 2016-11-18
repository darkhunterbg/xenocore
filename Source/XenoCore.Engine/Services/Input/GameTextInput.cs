using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Services.Input
{
    class GameTextInput : ITextInputProvider
    {
        public event Action<char> OnCharacterEntered;

        public GameTextInput(XenoCoreGame game)
        {
            game.Window.TextInput += (o, e) => OnCharacterEntered.Invoke(e.Character);
        }
    }
}
