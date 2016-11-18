using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Engine.Input
{
    public interface ITextInputProvider
    {
        event Action<Char> OnCharacterEntered;
    }
}
