using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Input
{
   public class EventTextInputProvider : ITextInputProvider
    {
        public void CharacterEntered(char e)
        {
            OnCharacterEntered?.Invoke(e);
        }

        public event Action<char> OnCharacterEntered;
    }
}
