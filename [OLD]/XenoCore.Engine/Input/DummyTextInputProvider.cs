using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Input
{
    public class DummyTextInputProvider : ITextInputProvider
    {
        public event Action<char> OnCharacterEntered;

        public DummyTextInputProvider()
        {
            OnCharacterEntered = null;
        }
    }
}
